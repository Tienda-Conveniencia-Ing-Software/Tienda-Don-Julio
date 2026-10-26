using TiendaBarrio.Core.Models;

namespace TiendaBarrio.Tests;

public class SaleTests
{
    [Fact]
    public void NuevaVenta_EmpiezaSinDetallesNiPersonas()
    {
        var sale = new Sale();

        Assert.Empty(sale.Details);
        Assert.Null(sale.ClienteId);   // null = venta de mostrador
        Assert.Null(sale.UsuarioId);
    }

    [Fact]
    public void CalculateTotals_ConDetalles_CalculaTotalCostoYGanancia()
    {
        var sale = new Sale();
        sale.Details.Add(new SaleDetail(1, "Arroz", 3, 2500, 1800));  // 7500 venta, 5400 costo
        sale.Details.Add(new SaleDetail(2, "Leche", 2, 3500, 2800));   // 7000 venta, 5600 costo

        sale.CalculateTotals();

        Assert.Equal(14500, sale.Total);
        Assert.Equal(11000, sale.TotalCost);
        Assert.Equal(3500, sale.Profit);
    }

    [Fact]
    public void CalculateTotals_SinDetalles_LanzaInvalidOperationException()
    {
        var sale = new Sale();

        Assert.Throws<InvalidOperationException>(() => sale.CalculateTotals());
    }
}
