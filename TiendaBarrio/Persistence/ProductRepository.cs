namespace TiendaBarrio.Persistence;

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using TiendaBarrio.Core.Models;

public  class ProductRepository
{
    private string RutaProductos = "C:\\Users\\PC\\source\\repos\\Tienda-Don-Julio\\TiendaBarrio\\Data\\productos.txt";

    public  List<Product> LoadProducts()
    {
        var products = new List<Product>();

        if (!File.Exists(RutaProductos))
            return products;

        foreach (string line in File.ReadLines(RutaProductos))
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            string[] lineSplit = line.Split(';');
            if (lineSplit.Length < 4)
                continue;

            if (!int.TryParse(lineSplit[0], out int id))
                continue;

            string name = lineSplit[1];

            // Normalize currency/number formats: remove currency symbol and thousand separators,
            // then ensure decimal separator is '.' for invariant parsing.
            string priceRaw = lineSplit[2].Replace("$", string.Empty).Trim();
            priceRaw = priceRaw.Replace(".", "").Replace(",", ".");
            if (!double.TryParse(priceRaw, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out double price))
                continue;

            if (!int.TryParse(lineSplit[3], out int stock))
                stock = 0;

            var p = new Product(id, name, price, stock);
            products.Add(p);
        }

        return products;
    }
    public  void SaveProducts(List<Product> products)
    {
        string[] lines = products
            .Select(p => $"{p.ID};{p.Name};{p.Price};{p.Stock}")
            .ToArray();
        File.WriteAllLines(RutaProductos, lines);
    }
}
