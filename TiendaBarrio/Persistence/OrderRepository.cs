namespace TiendaBarrio.Persistence;

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using TiendaBarrio.Core.Models;
using TiendaBarrio.Core.Config;

public class OrderRepository
{
    // Ruta centralizada en DataConfig
    private string RutaPedidos => DataConfig.PedidosFile;

    public void SaveOrder(Order order)
    {
        // Asegura que la carpeta Data exista
        DataConfig.EnsureDataFolderExists();

        // Format: OrderId;Status;CreatedAt;Total;ProductId:Qty,ProductId:Qty,...
        string itemsPart = string.Join(",", order.Items.Select(i => $"{i.Product.ID}:{i.Quantity}"));
        string line = $"{order.Id};{order.Status};{order.CreatedAt:yyyy-MM-dd HH:mm:ss};{order.Total.ToString(CultureInfo.InvariantCulture)};{itemsPart}";

        File.AppendAllLines(RutaPedidos, new[] { line });
    }

    public List<string> LoadOrderLines()
    {
        // Asegura que la carpeta Data exista
        DataConfig.EnsureDataFolderExists();

        if (!File.Exists(RutaPedidos))
            return new List<string>();

        return File.ReadAllLines(RutaPedidos).Where(l => !string.IsNullOrWhiteSpace(l)).ToList();
    }
}
