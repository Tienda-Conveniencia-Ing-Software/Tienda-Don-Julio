namespace TiendaBarrio.UI;

using TiendaBarrio.Core.Models;
using TiendaBarrio.Core.Services;

using TiendaBarrio.Persistence;
using TiendaBarrio.Persistence.Interfaces;
using TiendaBarrio.Utils;

public class MainMenu()
{
    private readonly CartService _cart = new();
    private readonly FinanceService _finance = new FinanceService(new CashRepository());
    private readonly IProductRepository _productRepository = new ProductRepository();
    private readonly IOrderRepository _orderRepository = new OrderRepository();
    private readonly ShowProducts _showProducts = new ShowProducts();
    private readonly FinanceMenu _financeMenu = new FinanceMenu();
    public void Start()
    {
        List<Product> products = new ProductRepository().LoadProducts();
        bool exit = true;
        while (exit)
        {
            Console.Clear();
            Console.WriteLine("Welcome to Don Julio´s store");
            ShopMenu();
            if (!int.TryParse(Console.ReadLine(), out int accion))
            {
                Console.WriteLine("Invalid input. Press any key... ");
                continue;
            }
            switch (accion)
            {
                case 0:
                    Console.WriteLine("Program break");
                    exit = false;
                    Console.WriteLine("Press any key...");
                    break;

                case 1:
                    _showProducts.StockMenu(products);
                    new Pause().pause();
                    break;

                case 2:
                    new SalesMenu(_cart, _finance).BuyStock(products);
                    new Pause().pause();
                    break;

                case 3:
                    new InventoryMenu(_finance).Start(products);
                    new Pause().pause();
                    break;

                case 4:
                    _financeMenu.Start();
                    new Pause().pause();
                    break;

                default:
                    Console.WriteLine("Option not available");
                    new Pause().pause();
                    break;
            }
        }
        static void ShopMenu()
        {
            Console.WriteLine("// OPEN PROGRAM \\\\");
            Console.WriteLine("0. Exit ");
            Console.WriteLine("1. see stock");
            Console.WriteLine("2. buy");
            Console.WriteLine("3. Inventory");
            Console.WriteLine("4. finance / balance");
            Console.WriteLine("\nSelect an option: ");
        }
    }
}