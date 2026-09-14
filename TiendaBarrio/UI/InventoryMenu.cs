namespace TiendaBarrio.UI;

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
                Console.Write($"[{p.ID}] ");
                Console.Write(p.Name);
                Console.Write(" | Sale: " + p.Price + "$ ");
                Console.Write("| Purchase: " + p.PurchasePrice + "$ ");
                Console.WriteLine("| Stock: " + p.Stock);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception: " + e.Message);
        }
    }
}
