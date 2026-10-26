using TiendaBarrio.Core.Models;

namespace TiendaBarrio.Tests;

public class CashMovementTests
{
    [Fact]
    public void Constructor_ConDatosValidos_AsignaLasPropiedades()
    {
        var m = new CashMovement(1, MovementType.Ingreso, 20500, "Venta #1");

        Assert.Equal(1, m.Id);
        Assert.Equal(MovementType.Ingreso, m.Type);
        Assert.Equal(20500, m.Amount);
        Assert.Equal("Venta #1", m.Description);
        Assert.Null(m.UsuarioId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Constructor_ConMontoCeroONegativo_LanzaArgumentException(double monto)
    {
        Assert.Throws<ArgumentException>(() => new CashMovement(1, MovementType.Gasto, monto, "Servicios"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_ConDescripcionVacia_LanzaArgumentException(string descripcion)
    {
        Assert.Throws<ArgumentException>(() => new CashMovement(1, MovementType.Gasto, 100, descripcion));
    }

    [Fact]
    public void ConstructorVacio_AsignaLaFechaActual()
    {
        var antes = DateTime.Now.AddSeconds(-5);

        var m = new CashMovement();

        Assert.InRange(m.Date, antes, DateTime.Now.AddSeconds(5));
    }
}
