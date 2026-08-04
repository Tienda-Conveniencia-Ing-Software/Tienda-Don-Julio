namespace TiendaBarrio.UI;

using TiendaBarrio.Persistence;
using TiendaBarrio.Core.Models;
using TiendaBarrio.Utils;
using TiendaBarrio.Inventario;
public class SalesMenu()
{
    public void BuyStock(List<Product> products)
    {
        try
        {
            new ShowProducts().StockMenu(products);
            Console.WriteLine("Put the ID of the product you are going to buy");
            int.TryParse(Console.ReadLine(), out int idfound);
            Product found = null;
            found = new InventoryService().FoundProduct(products, idfound);

            if (found != null)
            {
                Console.WriteLine("ID of the product found, the name is:" + found.Name);
                Console.WriteLine("The price of the product is: " + found.Price);
                Console.WriteLine("The skock of the product is: " + found.Stock);
                Console.WriteLine("How many do you want to buy?");
                bool valid = false;
                int quantity = 0;
                while (!valid)
                {
                    valid = int.TryParse(Console.ReadLine(), out quantity);
                    if (quantity > found.Stock)
                    { valid = false; }
                    if (!valid)
                    {
                        Console.WriteLine("Error: you have to put a number equal to ot less than the stock ");
                        Console.WriteLine("How many do you want to buy?");
                    }
                }
                found.ReduceStock(quantity);
                new ProductRepository().SaveProducts(products);
                Console.WriteLine("The new skock of the product is: " + found.Stock);
            }

        }
        catch (Exception e)
        {
            Console.WriteLine("Exception: " + e.Message);
        }

    }
}

