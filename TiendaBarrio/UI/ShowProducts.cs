namespace TiendaBarrio.UI;

using TiendaBarrio.Core.Models;

public class ShowProducts
{
    public void StockMenu(List<Product> products)
    {
        Console.WriteLine("== products ==");

        if (products.Count == 0)
        {
            Console.WriteLine("No products registered.");
            return;
        }

        foreach (var p in products)
        {
            Console.WriteLine($"[{p.ID}] {p.Name} - Sale: {p.Price}$ - Purchase: {p.PurchasePrice}$ - Stock: {p.Stock}");
        }
    }
}