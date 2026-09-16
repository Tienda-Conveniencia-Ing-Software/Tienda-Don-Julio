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
            Console.WriteLine("You want more stock or add new product?");
            bool exit = false;
            Console.WriteLine("0. Exit ");
            Console.WriteLine("1. Add stock");
            Console.WriteLine("2. Add new product");
            Console.WriteLine("\nSelect an option: ");

            exit = int.TryParse(Console.ReadLine(), out int option);

            while (exit)
            {
                if (option == 0)
                {
                    break;
                }

                if (option == 1)
                {
                    if (products.Count == 0)
                    {
                        Console.WriteLine("No hay productos para agregar stock. Primero cree un producto.");
                        break;
                    }

                    Console.WriteLine("Put the ID of the product you are searching");
                    int idfound;
                    while (!int.TryParse(Console.ReadLine(), out idfound))
                    {
                        Console.WriteLine("Error: You must enter a valid number for the ID.");
                        Console.WriteLine("Put the ID of the product you are searching");
                    }

                    Product found = FoundProduct(products, idfound);
                    if (found == null) { break; }

                    Console.WriteLine("Put the amount to add");
                    int quantity;
                    while (!int.TryParse(Console.ReadLine(), out quantity) || quantity <= 0)
                    {
                        Console.WriteLine("Error: You must enter a number greater than 0.");
                        Console.WriteLine("Put the amount to add");
                    }

                    found.IncreaseStock(quantity);

                    new ProductRepository().SaveProducts(products);

                    Console.WriteLine("The new stock of the product is: " + found.Stock);
                    break;
                }
                else if (option == 2)
                {
                    Console.WriteLine("Set name to the product");
                    string name = " " + Console.ReadLine() + " ";

                    int id = products.Count > 0 ? products[products.Count - 1].ID + 1 : 1;

                    Console.WriteLine("Set sale price to the product");
                    double price;
                    while (!double.TryParse(Console.ReadLine(), out price) || price <= 0)
                    {
                        Console.WriteLine("Error: You must enter a sale price greater than 0.");
                        Console.WriteLine("Set sale price to the product");
                    }

                    Console.WriteLine("Set purchase price to the product");
                    double purchasePrice;
                    while (!double.TryParse(Console.ReadLine(), out purchasePrice) || purchasePrice <= 0)
                    {
                        Console.WriteLine("Error: You must enter a purchase price greater than 0.");
                        Console.WriteLine("Set purchase price to the product");
                    }

                    Console.WriteLine("Set stock to the product");
                    int stock;
                    while (!int.TryParse(Console.ReadLine(), out stock) || stock < 0)
                    {
                        Console.WriteLine("Error: You must enter a stock greater than or equal to 0.");
                        Console.WriteLine("Set stock to the product");
                    }

                    Product p = new Product(id, name, price, purchasePrice, stock);
                    products.Add(p);

                    Console.WriteLine("The new product is:\n" +
                        "[" + p.ID + "] " + p.Name +
                        " Sale Price: " + p.Price + "$ " +
                        "Purchase Price: " + p.PurchasePrice + "$ " +
                        "Stock: " + p.Stock);

                    break;
                }
                else
                {
                    Console.WriteLine("Option not available");
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
