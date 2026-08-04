namespace TiendaBarrio.Utils;

using TiendaBarrio.Core.Models;
using TiendaBarrio.Persistence;
public class ShowProducts()
{
    public void StockMenu(List<Product> products)
    {
        try
        {

            Console.WriteLine("== products ==");
            foreach (Product p in products)
            {
                Console.Write($"[{p.ID}]");
                Console.Write(p.Name);
                Console.Write(p.Price + "$ ");
                Console.WriteLine(p.Stock);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception: " + e.Message);
        }
    }
 }