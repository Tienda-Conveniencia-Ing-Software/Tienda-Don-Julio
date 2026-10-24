namespace TiendaBarrio.Core.Models;

public class Sale
{
    public int Id { get; set; }
    public int? ClienteId { get; set; }   // comprador (null = venta de mostrador)
    public int? UsuarioId { get; set; }   // trabajador que procesó la venta
    public DateTime Date { get; set; }
    public List<SaleDetail> Details { get; set; }
    public double Total { get; set; }
    public double TotalCost { get; set; }
    public double Profit { get; set; }

    public Sale()
    {
        Details = new List<SaleDetail>();
        Date = DateTime.Now;
    }

    public void CalculateTotals()
    {
        if (Details.Count == 0)
        {
            throw new InvalidOperationException("No se puede calcular el total de una venta sin productos.");
        }

        Total = Details.Sum(d => d.Subtotal);
        TotalCost = Details.Sum(d => d.SubtotalCost);
        Profit = Total - TotalCost;
    }
}