namespace TiendaBarrio.Persistence;

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using TiendaBarrio.Core.Models;

public class ProductRepository
{
    private string RutaProductos = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Data", "productos.txt");

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

            if (!int.TryParse(lineSplit[0], out int id))
                continue;

            string name = lineSplit[1];

            // Normaliza el precio de venta
            string priceRaw = lineSplit[2].Replace("$", string.Empty).Trim();
            priceRaw = priceRaw.Replace(".", "").Replace(",", ".");
            if (!double.TryParse(priceRaw, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out double price))
                continue;

            // Leer el precio de compra (PurchasePrice)
            string purchasePriceRaw = lineSplit[3].Replace("$", string.Empty).Trim();
            purchasePriceRaw = purchasePriceRaw.Replace(".", "").Replace(",", ".");
            if (!double.TryParse(purchasePriceRaw, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out double purchasePrice))
                continue;

            if (!int.TryParse(lineSplit[4], out int stock))
                stock = 0;

            // Pasar purchasePrice al constructor
            var p = new Product(id, name, price, purchasePrice, stock);
            products.Add(p);
        }

        return products;
    }

    public void SaveProducts(List<Product> products)
    {
        // Guardar PurchasePrice en el archivo
        string[] lines = products
            .Select(p => $"{p.ID};{p.Name};{p.Price};{p.PurchasePrice};{p.Stock}")
            .ToArray();
        File.WriteAllLines(RutaProductos, lines);
    }
}
