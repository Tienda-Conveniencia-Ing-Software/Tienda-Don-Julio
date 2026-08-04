namespace TiendaBarrio.Inventario;
using TiendaBarrio.Core.Models;
using TiendaBarrio.Persistence;
using TiendaBarrio.Utils;

public class InventoryService()
{
    public Product FoundProduct(List<Product> products, int idfound)
    {
        Product found = null;
        found = products.FirstOrDefault(p => p.ID == idfound);
        if (found == null)
        {
            Console.WriteLine("ID product not found");
        }
        return found;
    }
    public void AddStock(List<Product> products)
    {
        try
        {
            Console.WriteLine("Existing stock");
            new ShowProducts().StockMenu(products);
            Console.WriteLine("You want more stock or add new produtc?");
            bool exit = false;
            Console.WriteLine("0. Exit ");
            Console.WriteLine("1. Add stock");
            Console.WriteLine("2. Add new produtc");
            Console.WriteLine("\nSelect an option: ");
            exit = int.TryParse(Console.ReadLine(), out int option);
            while (exit)
            {
                if (option == 0) { break; }

                if (option == 1)
                {
                    Console.WriteLine("Put the ID of the product you are searching");
                    int.TryParse(Console.ReadLine(), out int idfound);
                    Product found = null;
                    found = FoundProduct(products, idfound);
                    Console.WriteLine("Put the amount to add");
                    int.TryParse(Console.ReadLine(), out int quantity);
                    found.IncreaseStock(quantity);
                    new ProductRepository().SaveProducts(products);
                    Console.WriteLine("The new stock of the product is: " + found.Stock);
                    option = 0;
                }
                if (option == 2)
                {
                    Console.WriteLine("Set name to the product");
                    string name = " " + Console.ReadLine() + " ";
                    int id = products[products.Count - 1].ID + 1;
                    Console.WriteLine("Set price to the product");
                    double.TryParse(Console.ReadLine(), out double price);
                    Console.WriteLine("Set stock to the product");
                    int.TryParse(Console.ReadLine(), out int stock);
                    Product p = new Product(id, name, price, stock);
                    products.Add(p);
                    Console.WriteLine("The new product is:\n" + "[" +
                        products[products.Count - 1].ID + "] " +
                        products[products.Count - 1].Name + " " +
                        products[products.Count - 1].Price + "$ " +
                        products[products.Count - 1].Stock);
                    option = 0;
                }
                else
                {
                    Console.WriteLine("Option not available");
                    exit = false;
                    break;
                }

            }

            new ProductRepository().SaveProducts(products);
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception: " + e.Message);
        }
    }


}
