namespace TiendaBarrio.Persistence;

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using TiendaBarrio.Core.Models;
using TiendaBarrio.Core.Config;

public class ProductRepository
{
    // Ruta centralizada en DataConfig
    private string RutaProductos => DataConfig.ProductosFile;

    public List<Product> LoadProducts()
    {
        var products = new List<Product>();

        // Asegura que la carpeta Data exista
        DataConfig.EnsureDataFolderExists();

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

            // Normaliza el precio de venta
            string priceRaw = lineSplit[2].Replace("$", string.Empty).Trim();
            priceRaw = priceRaw.Replace(".", "").Replace(",", ".");
            if (!double.TryParse(priceRaw, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out double price))
                continue;

            // Normaliza el precio de compra
            string purchasePriceRaw = lineSplit[3].Replace("$", string.Empty).Trim();
            purchasePriceRaw = purchasePriceRaw.Replace(".", "").Replace(",", ".");
            if (!double.TryParse(purchasePriceRaw, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out double purchasePrice))
                continue;

            if (!int.TryParse(lineSplit[4], out int stock))
                stock = 0;

            var p = new Product(id, name, price, purchasePrice, stock);
            products.Add(p);
        }

        return products;
    }

    public void SaveProducts(List<Product> products)
    {
        // Asegura que la carpeta Data exista
        DataConfig.EnsureDataFolderExists();

        string[] lines = products
            .Select(p => $"{p.ID};{p.Name};{p.Price};{p.PurchasePrice};{p.Stock}")
            .ToArray();

        File.WriteAllLines(RutaProductos, lines);
    }
}
