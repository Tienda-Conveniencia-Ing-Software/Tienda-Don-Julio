namespace TiendaBarrio.Tests;

// Product no tiene namespace en el proyecto principal (está en el espacio global).
public class ProductTests
{
    [Fact]
    public void Constructor_ConValoresValidos_AsignaLasPropiedades()
    {
        var p = new Product(1, "Arroz", 2500, 1800, 100);

        Assert.Equal(1, p.ID);
        Assert.Equal("Arroz", p.Name);
        Assert.Equal(2500, p.Price);
        Assert.Equal(1800, p.PurchasePrice);
        Assert.Equal(100, p.Stock);
    }

    [Fact]
    public void Constructor_ConValoresNegativos_LosDejaEnCero()
    {
        var p = new Product(1, "Arroz", -5, -1, -3);

        Assert.Equal(0, p.Price);
        Assert.Equal(0, p.PurchasePrice);
        Assert.Equal(0, p.Stock);
    }

    [Fact]
    public void ReduceStock_ConCantidadValida_DescuentaDelStock()
    {
        var p = new Product(1, "Leche", 3500, 2800, 10);

        p.ReduceStock(3);

        Assert.Equal(7, p.Stock);
    }

    [Fact]
    public void ReduceStock_ConTodoElStock_DejaElStockEnCero()
    {
        var p = new Product(1, "Pan", 2000, 1500, 4);

        p.ReduceStock(4);

        Assert.Equal(0, p.Stock);
    }

    [Fact]
    public void ReduceStock_ConMasDelStockDisponible_NoCambiaElStock()
    {
        var p = new Product(1, "Huevos", 500, 350, 5);

        p.ReduceStock(10);

        Assert.Equal(5, p.Stock);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-2)]
    public void ReduceStock_ConCantidadCeroONegativa_NoCambiaElStock(int cantidad)
    {
        var p = new Product(1, "Sal", 1500, 1000, 8);

        p.ReduceStock(cantidad);

        Assert.Equal(8, p.Stock);
    }

    [Fact]
    public void IncreaseStock_ConCantidadPositiva_SumaAlStock()
    {
        var p = new Product(1, "Café", 8000, 6000, 30);

        p.IncreaseStock(20);

        Assert.Equal(50, p.Stock);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void IncreaseStock_ConCantidadCeroONegativa_NoCambiaElStock(int cantidad)
    {
        var p = new Product(1, "Azúcar", 3000, 2200, 60);

        p.IncreaseStock(cantidad);

        Assert.Equal(60, p.Stock);
    }
}
