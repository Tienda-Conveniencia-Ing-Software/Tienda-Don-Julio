namespace TiendaBarrio.Core.Services;

using TiendaBarrio.Core.Models;
using TiendaBarrio.Persistence;

public class OrderService
{
    private readonly OrderRepository _repository = new();
    private int _nextId = 1;

    public Order? ConfirmOrder(CartService cart, List<Product> products)
    {
        if (cart.Items.Count == 0)
        {
            Console.WriteLine("Cart is empty.");
            return null;
        }

        var order = new Order(_nextId, new List<CartItem>(cart.Items));
        _nextId++;

        foreach (var item in order.Items)
        {
            item.Product.ReduceStock(item.Quantity);
        }

        new ProductRepository().SaveProducts(products);
        order.AdvanceStatus(OrderStatus.Confirmado);
        _repository.SaveOrder(order);

        cart.Clear();
        return order;
    }

    public void ShowHistory(List<Product> products)
    {
        var lines = _repository.LoadOrderLines();
        if (lines.Count == 0)
        {
            Console.WriteLine("No orders yet.");
            return;
        }

        Console.WriteLine("Order history:");
        foreach (var line in lines)
        {
            string[] parts = line.Split(';');
            if (parts.Length < 5) continue;

            string id = parts[0];
            string status = parts[1];
            string date = parts[2];
            string total = parts[3];
            string itemsRaw = parts[4];

            Console.WriteLine($"\nOrder #{id} - {status} - {date} - Total: {total}");

            var itemPairs = itemsRaw.Split(',');
            foreach (var pair in itemPairs)
            {
                string[] idQty = pair.Split(':');
                if (idQty.Length < 2) continue;

                if (!int.TryParse(idQty[0], out int productId)) continue;
                if (!int.TryParse(idQty[1], out int quantity)) continue;

                var product = products.FirstOrDefault(p => p.ID == productId);
            string name = product != null ? product.Name : $"Product #{productId} (not found)";

                Console.WriteLine($"  - {name} x{quantity}");
            }
        }
    }
}