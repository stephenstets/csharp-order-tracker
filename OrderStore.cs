using System.Text.Json;

namespace OrderTracker;

/// <summary>
/// Loads and saves the order list as JSON. No database — orders.json is the
/// database, and it opens in any text editor.
/// </summary>
public class OrderStore
{
    private readonly string _path;
    private readonly JsonSerializerOptions _json = new() { WriteIndented = true };

    public List<Order> Orders { get; private set; } = new();

    public OrderStore(string path) => _path = path;

    public void Load()
    {
        if (!File.Exists(_path)) return;
        var text = File.ReadAllText(_path);
        Orders = JsonSerializer.Deserialize<List<Order>>(text, _json) ?? new();
    }

    public void Save() =>
        File.WriteAllText(_path, JsonSerializer.Serialize(Orders, _json));

    public Order Add(string customer, string product, int quantity, decimal unitPrice)
    {
        var nextId = Orders.Count == 0 ? 1 : Orders.Max(o => o.Id) + 1;
        var order = new Order
        {
            Id = nextId,
            Customer = customer,
            Product = product,
            Quantity = quantity,
            UnitPrice = unitPrice,
        };
        Orders.Add(order);
        return order;
    }

    public Order? Find(int id) => Orders.FirstOrDefault(o => o.Id == id);

    public IEnumerable<Order> Pending() => Orders.Where(o => o.Status == "Pending");

    public decimal PendingValue() => Pending().Sum(o => o.Total);

    public decimal ShippedValue() =>
        Orders.Where(o => o.Status == "Shipped").Sum(o => o.Total);
}
