namespace TiendaBarrio.Core.Models;

public class Sale
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public List<SaleDetail> Details { get; set; }
    public double Total { get; set; }        // Total de la venta (precio de venta)
    public double TotalCost { get; set; }    // Costo total (precio de compra)
    public double Profit { get; set; }       // Ganancia = Total - TotalCost

    public Sale()
    {
        Details = new List<SaleDetail>();
        Date = DateTime.Now;
    }

    // Calcula el total, el costo y la ganancia a partir de los detalles
    public void CalculateTotals()
    {
        // Validación: No se puede calcular totales de una venta sin productos
        if (Details.Count == 0)
        {
            throw new InvalidOperationException("No se puede calcular el total de una venta sin productos.");
        }

        Total = Details.Sum(d => d.Subtotal);
        TotalCost = Details.Sum(d => d.SubtotalCost);
        Profit = Total - TotalCost;
    }
}
