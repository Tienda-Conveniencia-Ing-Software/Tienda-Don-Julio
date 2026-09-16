namespace TiendaBarrio.UI;

using TiendaBarrio.Persistence;
using TiendaBarrio.Core.Models;
using TiendaBarrio.Core.Services;
using TiendaBarrio.Utils;
using TiendaBarrio.Inventario;
using TiendaBarrio.Modules.Ventas;

public class SalesMenu(CartService cart)
{
    private readonly CartService _cart = cart;
    private readonly SaleService _saleService = new();

    public void BuyStock(List<Product> products)
    {
        bool exit = true;
        while (exit)
        {
            Console.Clear();
            Console.WriteLine("// BUY \\\\");
            Console.WriteLine("0. Back");
            Console.WriteLine("1. Add product to cart");
            Console.WriteLine("2. View / edit cart");
            Console.WriteLine("3. Checkout (pay)");
            Console.WriteLine("\nSelect an option: ");

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
                    new CartMenu(_cart).Start(products);
                    break;

                case 3:
                    CheckoutFlow(products);
                    new Pause().pause();
                    break;

                default:
                    Console.WriteLine("Option not available");
                    new Pause().pause();
                    break;
            }
        }
    }

    private void AddProductFlow(List<Product> products)
    {
        try
        {
            new ShowProducts().StockMenu(products);
            Console.WriteLine("Put the ID of the product you are going to buy");
            int idfound;
            while (!int.TryParse(Console.ReadLine(), out idfound))
            {
                Console.WriteLine("Error: You must enter a valid number for the ID.");
                Console.WriteLine("Put the ID of the product you are going to buy");
            }

            Product found = new InventoryService().FoundProduct(products, idfound);

            if (found == null)
            {
                Console.WriteLine("Product not found.");
                return;
            }

            Console.WriteLine("ID of the product found, the name is:" + found.Name);
            Console.WriteLine("The sale price of the product is: " + found.Price);
            Console.WriteLine("The stock of the product is: " + found.Stock);
            Console.WriteLine("How many do you want to buy?");

            bool valid = false;
            int quantity = 0;
            while (!valid)
            {
                string input = Console.ReadLine();

                if (!int.TryParse(input, out quantity))
                {
                    Console.WriteLine("Error: You must enter a valid number.");
                    Console.WriteLine("How many do you want to buy?");
                    continue;
                }

                if (quantity <= 0 || quantity > found.Stock)
                {
                    Console.WriteLine($"Error: Quantity must be greater than 0 and not exceed available stock ({found.Stock}).");
                    Console.WriteLine("How many do you want to buy?");
                    continue;
                }

                valid = true;
            }

            _cart.AddProduct(found, quantity);
            Console.WriteLine("Added to cart. Go to 'Checkout' when ready to pay.");
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception: " + e.Message);
        }
    }

    private void CheckoutFlow(List<Product> products)
    {
        if (_cart.Items.Count == 0)
        {
            Console.WriteLine("Your cart is empty. Add products before checkout.");
            return;
        }

        Console.WriteLine("== Payment gateway ==");
        foreach (var item in _cart.Items)
        {
            Console.WriteLine($"{item.Product.Name} x{item.Quantity} = {item.Subtotal}");
        }
        Console.WriteLine($"Total to pay: {_cart.CalculateTotal()}");
        Console.WriteLine("Confirm payment? (y/n)");

        string? answer = Console.ReadLine();
        if (answer?.Trim().ToLower() != "y")
        {
            Console.WriteLine("Payment cancelled.");
            return;
        }

        // INTEGRACIÓN CON SALE SERVICE
        var sale = _saleService.ProcessSale(_cart, products);

        if (sale != null)
        {
            Console.WriteLine($"Payment successful. Order #{sale.Id} confirmed.");
            Console.WriteLine($"   Total: {sale.Total}$ | Cost: {sale.TotalCost}$ | Profit: {sale.Profit}$");
        }
        else
        {
            Console.WriteLine("Payment failed. Please check the errors above.");
        }
    }
}
