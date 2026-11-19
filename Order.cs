namespace OrderTracker;

/// <summary>One customer order in a small shop.</summary>
public class Order
{
    public int Id { get; set; }
    public string Customer { get; set; } = "";
    public string Product { get; set; } = "";
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string Status { get; set; } = "Pending"; // Pending | Shipped
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? ShippedAt { get; set; }

    public decimal Total => Quantity * UnitPrice;

    public override string ToString() =>
        $"#{Id,-3} {Customer,-16} {Product,-28} x{Quantity,-3} @ {UnitPrice,6:C} = {Total,7:C}  [{Status}]";
}
