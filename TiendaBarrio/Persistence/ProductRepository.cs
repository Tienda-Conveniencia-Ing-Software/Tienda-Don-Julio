namespace TiendaBarrio.Persistence;

using System.Globalization;
using TiendaBarrio.Core.Models;

public class ProductRepository
{
    private readonly string RutaProductos =
        Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Data", "productos.txt");

    public List<Product> LoadProducts()
    {
        var products = new List<Product>();

        if (!File.Exists(RutaProductos))
            return products;

        foreach (string line in File.ReadLines(RutaProductos))
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            string[] lineSplit = line.Split(';');

            if (lineSplit.Length < 5)
                continue;

            if (!int.TryParse(lineSplit[0].Trim(), out int id))
                continue;

            string name = lineSplit[1].Trim();

            if (!double.TryParse(
                    lineSplit[2].Trim(),
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out double sprice))
                continue;

            if (!double.TryParse(
                    lineSplit[3].Trim(),
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out double bprice))
                continue;

            if (!int.TryParse(lineSplit[4].Trim(), out int stock))
                stock = 0;

            var product = new Product(id, name, sprice, bprice, stock);
            products.Add(product);
        }

        return products;
    }

    public void SaveProducts(List<Product> products)
    {
        string[] lines = products
            .Select(p => $"{p.ID};{p.Name};{p.SPrice};{p.BPrice};{p.Stock}")
            .ToArray();

        File.WriteAllLines(RutaProductos, lines);
    }
}