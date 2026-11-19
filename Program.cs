namespace OrderTracker;

using System.Globalization;

/// <summary>
/// Order tracker for a small shop — console app.
///
/// Commands are typed at the prompt; type "help" for the list. Orders are
/// stored in orders.json and saved after every change.
///
/// Build: dotnet build
/// Run:   dotnet run
/// </summary>
internal static class Program
{
    private const string Help = """
        Commands:
          add              add a new order (prompts for each field)
          list             show all orders
          pending          show unshipped orders
          ship <id>        mark an order shipped
          show <id>        show one order in detail
          totals           pending vs shipped totals
          help             this list
          quit             save and exit
        """;

    private static void Main()
    {
        // Pin currency formatting to US dollars so prices print as $35.00
        // on any machine, whatever its system locale is.
        CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");

        var store = new OrderStore("orders.json");
        store.Load();
        Console.WriteLine($"Loaded {store.Orders.Count} order(s) from orders.json.");
        Console.WriteLine("Order tracker — type 'help' for commands.");

        while (true)
        {
            Console.Write("> ");
            var line = Console.ReadLine()?.Trim() ?? "";
            if (line.Length == 0) continue;
            var parts = line.Split(' ', 2, StringSplitOptions.TrimEntries);
            var cmd = parts[0].ToLowerInvariant();
            var arg = parts.Length > 1 ? parts[1] : "";

            switch (cmd)
            {
                case "add": AddOrder(store); break;
                case "list": ListOrders(store.Orders, "All orders"); break;
                case "pending": ListOrders(store.Pending(), "Pending orders"); break;
                case "ship": ShipOrder(store, arg); break;
                case "show": ShowOrder(store, arg); break;
                case "totals":
                    Console.WriteLine($"Pending: {store.PendingValue():C} | Shipped: {store.ShippedValue():C}");
                    break;
                case "help": Console.WriteLine(Help); break;
                case "quit":
                case "exit":
                    store.Save();
                    Console.WriteLine("Saved. Goodbye.");
                    return;
                default:
                    Console.WriteLine("Unknown command — type 'help'.");
                    break;
            }
        }
    }

    private static void AddOrder(OrderStore store)
    {
        var customer = Prompt("Customer name: ");
        var product = Prompt("Product: ");
        var quantity = PromptInt("Quantity: ");
        var price = PromptDecimal("Unit price ($): ");

        var order = store.Add(customer, product, quantity, price);
        store.Save();
        Console.WriteLine($"Added: {order}");
    }

    private static void ListOrders(IEnumerable<Order> orders, string heading)
    {
        var list = orders.OrderBy(o => o.Id).ToList();
        Console.WriteLine($"--- {heading} ({list.Count}) ---");
        if (list.Count == 0)
        {
            Console.WriteLine("(none)");
            return;
        }
        foreach (var order in list) Console.WriteLine(order);
    }

    private static void ShipOrder(OrderStore store, string arg)
    {
        if (!int.TryParse(arg, out var id))
        {
            Console.WriteLine("Usage: ship <id>");
            return;
        }
        var order = store.Find(id);
        if (order is null)
        {
            Console.WriteLine($"No order #{id}.");
            return;
        }
        if (order.Status == "Shipped")
        {
            Console.WriteLine($"Order #{id} already shipped on {order.ShippedAt:yyyy-MM-dd}.");
            return;
        }
        order.Status = "Shipped";
        order.ShippedAt = DateTime.Now;
        store.Save();
        Console.WriteLine($"Shipped: {order}");
    }

    private static void ShowOrder(OrderStore store, string arg)
    {
        if (!int.TryParse(arg, out var id))
        {
            Console.WriteLine("Usage: show <id>");
            return;
        }
        var order = store.Find(id);
        if (order is null)
        {
            Console.WriteLine($"No order #{id}.");
            return;
        }
        Console.WriteLine($"Order #{order.Id}");
        Console.WriteLine($"  Customer:  {order.Customer}");
        Console.WriteLine($"  Product:   {order.Product} x{order.Quantity} @ {order.UnitPrice:C}");
        Console.WriteLine($"  Total:     {order.Total:C}");
        Console.WriteLine($"  Status:    {order.Status}");
        Console.WriteLine($"  Created:   {order.CreatedAt:yyyy-MM-dd HH:mm}");
        if (order.ShippedAt is not null)
            Console.WriteLine($"  Shipped:   {order.ShippedAt:yyyy-MM-dd HH:mm}");
    }

    private static string Prompt(string text)
    {
        while (true)
        {
            Console.Write(text);
            var value = Console.ReadLine()?.Trim() ?? "";
            if (value.Length > 0) return value;
            Console.WriteLine("Can't be empty — try again.");
        }
    }

    private static int PromptInt(string text)
    {
        while (true)
        {
            Console.Write(text);
            if (int.TryParse(Console.ReadLine(), out var value) && value > 0) return value;
            Console.WriteLine("Enter a whole number above 0.");
        }
    }

    private static decimal PromptDecimal(string text)
    {
        while (true)
        {
            Console.Write(text);
            if (decimal.TryParse(Console.ReadLine(), out var value) && value >= 0) return value;
            Console.WriteLine("Enter a dollar amount, e.g. 35.00.");
        }
    }
}
