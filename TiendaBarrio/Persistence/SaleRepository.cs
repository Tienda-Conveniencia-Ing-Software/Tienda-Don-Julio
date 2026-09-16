namespace TiendaBarrio.Persistence;

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using TiendaBarrio.Core.Models;
using TiendaBarrio.Core.Config;

public class SaleRepository
{
    // Ruta centralizada en DataConfig
    private string RutaVentas => DataConfig.VentasFile;

    public List<Sale> LoadSales()
    {
        var sales = new List<Sale>();

        // Asegura que la carpeta Data exista
        DataConfig.EnsureDataFolderExists();

        if (!File.Exists(RutaVentas))
            return sales;

        foreach (string line in File.ReadLines(RutaVentas))
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            string[] parts = line.Split('|');
            if (parts.Length < 5)
                continue;

            if (!int.TryParse(parts[0], out int id))
                continue;

            if (!DateTime.TryParse(parts[1], out DateTime date))
                continue;

            if (!double.TryParse(parts[2], NumberStyles.Any, CultureInfo.InvariantCulture, out double total))
                continue;

            if (!double.TryParse(parts[3], NumberStyles.Any, CultureInfo.InvariantCulture, out double totalCost))
                continue;

            if (!double.TryParse(parts[4], NumberStyles.Any, CultureInfo.InvariantCulture, out double profit))
                continue;

            var sale = new Sale
            {
                Id = id,
                Date = date,
                Total = total,
                TotalCost = totalCost,
                Profit = profit
            };

            for (int i = 5; i < parts.Length; i++)
            {
                string[] detailParts = parts[i].Split(',');
                if (detailParts.Length < 5)
                    continue;

                if (!int.TryParse(detailParts[0], out int productId))
                    continue;

                string productName = detailParts[1];

                if (!int.TryParse(detailParts[2], out int quantity))
                    continue;

                if (!double.TryParse(detailParts[3], NumberStyles.Any, CultureInfo.InvariantCulture, out double unitPrice))
                    continue;

                if (!double.TryParse(detailParts[4], NumberStyles.Any, CultureInfo.InvariantCulture, out double unitCost))
                    continue;

                var detail = new SaleDetail(productId, productName, quantity, unitPrice, unitCost);
                sale.Details.Add(detail);
            }

            sales.Add(sale);
        }

        return sales;
    }

    public void SaveSale(Sale sale)
    {
        if (sale == null)
        {
            Console.WriteLine("❌ Error: No se puede guardar una venta nula.");
            return;
        }

        if (sale.Details == null || sale.Details.Count == 0)
        {
            Console.WriteLine("❌ Error: No se puede guardar una venta sin productos.");
            return;
        }

        try
        {
            // Asegura que la carpeta Data exista
            DataConfig.EnsureDataFolderExists();

            string details = string.Join("|", sale.Details.Select(d =>
                $"{d.ProductId},{d.ProductName},{d.Quantity},{d.UnitPrice},{d.UnitCost}"));

            string line = $"{sale.Id};{sale.Date:yyyy-MM-dd HH:mm:ss};{sale.Total};{sale.TotalCost};{sale.Profit}|{details}";

            if (!File.Exists(RutaVentas))
            {
                File.WriteAllText(RutaVentas, line + Environment.NewLine);
            }
            else
            {
                File.AppendAllText(RutaVentas, line + Environment.NewLine);
            }

            Console.WriteLine("Venta guardada correctamente.");
        }
        catch (Exception e)
        {
            Console.WriteLine("Error al guardar la venta: " + e.Message);
        }
    }

    public int GetNextSaleId()
    {
        var sales = LoadSales();
        return sales.Count > 0 ? sales.Max(s => s.Id) + 1 : 1;
    }
}
