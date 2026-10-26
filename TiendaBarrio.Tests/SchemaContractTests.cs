using TiendaBarrio.Core.Models;

namespace TiendaBarrio.Tests;

/// <summary>
/// Protege que el código y el script SQL no se desalineen: los estados, tipos y roles
/// que usa C# deben estar permitidos por los CHECK de database/init/01-schema.sql.
/// </summary>
public class SchemaContractTests
{
    private static string LeerEsquema()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null && !File.Exists(Path.Combine(dir.FullName, "TiendaBarrio.sln")))
        {
            dir = dir.Parent;
        }

        Assert.NotNull(dir);

        var ruta = Path.Combine(dir!.FullName, "database", "init", "01-schema.sql");
        Assert.True(File.Exists(ruta), $"No se encontró el script de esquema en {ruta}");

        return File.ReadAllText(ruta);
    }

    [Theory]
    [InlineData(OrderStatus.Pendiente)]
    [InlineData(OrderStatus.EnProceso)]
    [InlineData(OrderStatus.Completado)]
    [InlineData(OrderStatus.Cancelado)]
    public void EstadoDePedido_EstaPermitidoEnElEsquemaSql(OrderStatus estado)
    {
        Assert.Contains($"'{estado}'", LeerEsquema());
    }

    [Theory]
    [InlineData(MovementType.Ingreso)]
    [InlineData(MovementType.Gasto)]
    [InlineData(MovementType.CompraInventario)]
    public void TipoDeMovimiento_EstaPermitidoEnElEsquemaSql(MovementType tipo)
    {
        Assert.Contains($"'{tipo}'", LeerEsquema());
    }

    [Theory]
    [InlineData("Cliente")]
    [InlineData("Trabajador")]
    public void RolDeCuenta_EstaPermitidoEnElEsquemaSql(string rol)
    {
        Assert.Contains($"'{rol}'", LeerEsquema());
    }

    [Fact]
    public void LosEnumsNoTienenValoresSinCubrirPorLasPruebas()
    {
        // Si agregas un estado o tipo nuevo, esta prueba falla para recordarte
        // actualizar el CHECK del script SQL y las pruebas de arriba.
        Assert.Equal(4, Enum.GetValues<OrderStatus>().Length);
        Assert.Equal(3, Enum.GetValues<MovementType>().Length);
    }
}
