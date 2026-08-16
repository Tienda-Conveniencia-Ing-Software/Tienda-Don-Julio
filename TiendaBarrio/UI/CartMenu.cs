namespace TiendaBarrio.UI;

using TiendaBarrio.Core.Models;
using TiendaBarrio.Core.Services;
using TiendaBarrio.Utils;

public class CartMenu
{
    private readonly CartService _cart = new();
    private readonly OrderService _orderService = new();

    public void Start(List<Product> products)
    {
        bool exit = true;
        while (exit)
        {
            Console.Clear();
            ShowCart();
            ShowMenu();

            if (!int.TryParse(Console.ReadLine(), out int option))
            {
                Console.WriteLine("Invalid input.");
                new Pause().pause();
                continue;
            }

            switch (option)
            {
                case 0:
                    exit = false;
                    break;

                case 1:
                    AddProductFlow(products);
                    new Pause().pause();
                    break;

                case 2:
                    UpdateQuantityFlow();
                    new Pause().pause();
                    break;

                case 3:
                    RemoveItemFlow();
                    new Pause().pause();
                    break;

                case 4:
                    ConfirmOrderFlow(products);
                    new Pause().pause();
                    break;

                case 5:
                    _orderService.ShowHistory(products);
                    new Pause().pause();
                    break;

                default:
                    Console.WriteLine("Option not available");
                    new Pause().pause();
                    break;
            }
        }
    }

    private void ShowMenu()
    {
        Console.WriteLine("\n// CART \\\\");
        Console.WriteLine("0. Back");
        Console.WriteLine("1. Add product");
        Console.WriteLine("2. Update quantity");
        Console.WriteLine("3. Remove item");
        Console.WriteLine("4. Confirm order");
        Console.WriteLine("5. Order history");
        Console.WriteLine("\nSelect an option: ");
    }

    private void ShowCart()
    {
        Console.WriteLine("Current cart:");
        if (_cart.Items.Count == 0)
        {
            Console.WriteLine("(empty)");
        }
        else
        {
            foreach (var item in _cart.Items)
            {
                Console.WriteLine($"{item.Product.ID} - {item.Product.Name} x{item.Quantity} = {item.Subtotal}");
            }
            Console.WriteLine($"Total: {_cart.CalculateTotal()}");
        }
    }

    private void AddProductFlow(List<Product> products)
    {
        Console.WriteLine("Enter product ID:");
        if (!int.TryParse(Console.ReadLine(), out int id)) return;

        var product = products.FirstOrDefault(p => p.ID == id);
        if (product == null)
        {
            Console.WriteLine("Product not found.");
            return;
        }

        Console.WriteLine("Enter quantity:");
        if (!int.TryParse(Console.ReadLine(), out int quantity)) return;

        _cart.AddProduct(product, quantity);
    }

    private void UpdateQuantityFlow()
    {
        Console.WriteLine("Enter product ID:");
        if (!int.TryParse(Console.ReadLine(), out int id)) return;

        Console.WriteLine("Enter new quantity:");
        if (!int.TryParse(Console.ReadLine(), out int quantity)) return;

        _cart.UpdateQuantity(id, quantity);
    }

    private void RemoveItemFlow()
    {
        Console.WriteLine("Enter product ID to remove:");
        if (!int.TryParse(Console.ReadLine(), out int id)) return;

        _cart.RemoveItem(id);
    }

    private void ConfirmOrderFlow(List<Product> products)
    {
        var order = _orderService.ConfirmOrder(_cart, products);
        if (order != null)
        {
            Console.WriteLine($"Order #{order.Id} confirmed. Total: {order.Total}. Status: {order.Status}");
        }
    }
}