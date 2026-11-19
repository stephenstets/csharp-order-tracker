# csharp-order-tracker

A console order tracker for a small shop, in C# (.NET 8). Add, list, and ship
orders; everything is stored in `orders.json` — no database, no NuGet
packages, just the standard library (`System.Text.Json`).

## Build and run

```bash
dotnet build
dotnet run
```

## Commands

```
add              add a new order (prompts for each field)
list             show all orders
pending          show unshipped orders
ship <id>        mark an order shipped
show <id>        show one order in detail
totals           pending vs shipped totals
quit             save and exit
```

## Example session

```
> add
Customer name: Dana
Product: Golf cart name plate
Quantity: 1
Unit price ($): 35.00
Added: #1   Dana             Golf cart name plate         x1   @ $35.00 =  $35.00  [Pending]

> ship 1
Shipped: #1   Dana             Golf cart name plate         x1   @ $35.00 =  $35.00  [Shipped]

> totals
Pending: $0.00 | Shipped: $35.00
```

## How it's put together

| File | What it does |
|---|---|
| `Order.cs` | One order: fields, computed `Total`, display format |
| `OrderStore.cs` | The list: JSON load/save, add/find, pending + totals queries |
| `Program.cs` | The command loop and all user interaction |

Data lives in `orders.json` (created on first save, gitignored). Every change
saves immediately.

## License

MIT — see `LICENSE`.
