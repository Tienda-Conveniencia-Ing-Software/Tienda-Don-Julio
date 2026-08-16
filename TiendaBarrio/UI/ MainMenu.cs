namespace TiendaBarrio.UI;

using TiendaBarrio.Core.Models;
using TiendaBarrio.Inventario;
using TiendaBarrio.Persistence;
using TiendaBarrio.Utils;

public class MainMenu(List<Product> products)
{
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
                    new ShowProducts().StockMenu(products);
                    new Pause().pause();
                    break;

                case 2:
                    new SalesMenu().BuyStock(products);
                    new Pause().pause();
                    break;

                case 3:
                    new InventoryService().AddStock(products);
                    new Pause().pause();
                    break;

                case 4:
                    new CartMenu().Start(products);
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
            Console.WriteLine("3. add stock");
            Console.WriteLine("4. cart / checkout");
            Console.WriteLine("\nSelect an option: ");
        }

    }
}