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

    public void ShowHistory()
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
            Console.WriteLine(line);
        }
    }
}