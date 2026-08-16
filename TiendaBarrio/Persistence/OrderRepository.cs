namespace TiendaBarrio.Persistence;

using System.Globalization;
using TiendaBarrio.Core.Models;

public class OrderRepository
{
    private string RutaPedidos = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Data", "pedidos.txt");

    public void SaveOrder(Order order)
    {
        // Format: OrderId;Status;CreatedAt;Total;ProductId:Qty,ProductId:Qty,...
        string itemsPart = string.Join(",", order.Items.Select(i => $"{i.Product.ID}:{i.Quantity}"));
        string line = $"{order.Id};{order.Status};{order.CreatedAt:yyyy-MM-dd HH:mm:ss};{order.Total.ToString(CultureInfo.InvariantCulture)};{itemsPart}";

        File.AppendAllLines(RutaPedidos, new[] { line });
    }

    public List<string> LoadOrderLines()
    {
        if (!File.Exists(RutaPedidos))
            return new List<string>();

        return File.ReadAllLines(RutaPedidos).Where(l => !string.IsNullOrWhiteSpace(l)).ToList();
    }
}