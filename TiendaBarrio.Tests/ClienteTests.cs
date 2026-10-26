using TiendaBarrio.Core.Models;

namespace TiendaBarrio.Tests;

public class ClienteTests
{
    [Fact]
    public void Constructor_ConDatosValidos_AsignaLasPropiedades()
    {
        var c = new Cliente(1, "Juan Perez", "12345678", "3001234567", "juan@test.com", "hash", "Cliente");

        Assert.Equal(1, c.Id);
        Assert.Equal("Juan Perez", c.Nombre);
        Assert.Equal("12345678", c.Cedula);
        Assert.Equal("juan@test.com", c.Email);
        Assert.Equal("hash", c.PasswordHash);
        Assert.Equal("Cliente", c.Role);
    }

    [Fact]
    public void ConstructorVacio_TieneRolClientePorDefecto()
    {
        var c = new Cliente();

        Assert.Equal("Cliente", c.Role);
    }

    [Fact]
    public void Constructor_SinRol_UsaCliente()
    {
        var c = new Cliente(1, "Ana", "44332211", "3004433221", "ana@test.com", "hash");

        Assert.Equal("Cliente", c.Role);
    }

    [Fact]
    public void Constructor_ConRolEnBlanco_UsaCliente()
    {
        var c = new Cliente(1, "Ana", "44332211", "3004433221", "ana@test.com", "hash", "  ");

        Assert.Equal("Cliente", c.Role);
    }

    [Theory]
    [InlineData("", "123", "hash")]        // nombre vacío
    [InlineData("Ana", "", "hash")]        // cédula vacía
    [InlineData("Ana", "123", "")]         // hash vacío
    [InlineData("   ", "123", "hash")]     // nombre en blanco
    public void Constructor_ConCamposObligatoriosVacios_LanzaArgumentException(string nombre, string cedula, string hash)
    {
        Assert.Throws<ArgumentException>(() => new Cliente(1, nombre, cedula, "300", "a@test.com", hash));
    }

    [Theory]
    [InlineData("Trabajador", true)]
    [InlineData("trabajador", true)]
    [InlineData("TRABAJADOR", true)]
    [InlineData("Cliente", false)]
    [InlineData("Otro", false)]
    public void EsTrabajador_DependeDelRol_SinImportarMayusculas(string rol, bool esperado)
    {
        var c = new Cliente(1, "Carlos", "11223344", "3001122334", "carlos@test.com", "hash", rol);

        Assert.Equal(esperado, c.EsTrabajador());
    }
}
