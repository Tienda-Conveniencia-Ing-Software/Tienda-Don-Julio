namespace TiendaBarrio.UI;

using TiendaBarrio.Core.Models;
using TiendaBarrio.Core.Services;
using TiendaBarrio.Persistence;
using TiendaBarrio.Persistence.Interfaces;
using TiendaBarrio.Utils;

public class MainMenu(
    IProductRepository productRepository,
    ShowProducts showProducts,
    SalesMenu salesMenu,
    InventoryMenu inventoryMenu,
    FinanceMenu financeMenu)
{
    private readonly IProductRepository _productRepository = productRepository;
    private readonly ShowProducts _showProducts = showProducts;
    private readonly SalesMenu _salesMenu = salesMenu;
    private readonly InventoryMenu _inventoryMenu = inventoryMenu;
    private readonly FinanceMenu _financeMenu = financeMenu;

    public async Task StartAsync()
    {
        List<Product> products = await _productRepository.GetAllAsync();
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
                    await _salesMenu.BuyStockAsync(products);
                    new Pause().pause();
                    break;

                case 3:
                    await _inventoryMenu.StartAsync(products);
                    new Pause().pause();
                    break;

                case 4:
                    await _financeMenu.StartAsync();
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