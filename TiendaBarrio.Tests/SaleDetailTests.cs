using TiendaBarrio.Core.Models;

namespace TiendaBarrio.Tests;

public class SaleDetailTests
{
    [Fact]
    public void Constructor_ConDatosValidos_CalculaSubtotales()
    {
        var d = new SaleDetail(1, "Arroz", 3, 2500, 1800);

        Assert.Equal(1, d.ProductId);
        Assert.Equal("Arroz", d.ProductName);
        Assert.Equal(3, d.Quantity);
        Assert.Equal(7500, d.Subtotal);
        Assert.Equal(5400, d.SubtotalCost);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_ConCantidadCeroONegativa_LanzaArgumentException(int cantidad)
    {
        Assert.Throws<ArgumentException>(() => new SaleDetail(1, "Arroz", cantidad, 2500, 1800));
    }

    [Fact]
    public void Constructor_ConPrecioDeVentaNegativo_LanzaArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new SaleDetail(1, "Arroz", 1, -1, 1800));
    }

    [Fact]
    public void Constructor_ConCostoNegativo_LanzaArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new SaleDetail(1, "Arroz", 1, 2500, -1));
    }

    [Fact]
    public void Constructor_ConPrecioCero_EsValido()
    {
        var d = new SaleDetail(1, "Obsequio", 2, 0, 0);

        Assert.Equal(0, d.Subtotal);
        Assert.Equal(0, d.SubtotalCost);
    }
}
