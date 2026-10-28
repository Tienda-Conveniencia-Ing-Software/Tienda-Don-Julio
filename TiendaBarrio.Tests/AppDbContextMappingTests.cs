using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using TiendaBarrio.Core.Models;
using TiendaBarrio.Persistence.Data;

namespace TiendaBarrio.Tests;

/// <summary>
/// Comprueba el contrato de nombres entre las entidades EF Core y el esquema SQL.
/// No necesita una conexión activa a PostgreSQL.
/// </summary>
public class AppDbContextMappingTests
{
    private static readonly AppDbContext Context = new();

    private static string? Table<TEntity>() where TEntity : class =>
        Context.Model.FindEntityType(typeof(TEntity))?.GetTableName();

    private static string? Column<TEntity>(string property) where TEntity : class
    {
        var entity = Context.Model.FindEntityType(typeof(TEntity));
        if (entity == null) return null;

        var table = entity.GetTableName();
        if (table == null) return null;

        var storeObject = StoreObjectIdentifier.Table(table, entity.GetSchema());
        return entity.FindProperty(property)?.GetColumnName(storeObject);
    }

    [Theory]
    [InlineData(typeof(Cliente), "cliente")]
    [InlineData(typeof(Product), "producto")]
    [InlineData(typeof(Order), "pedido")]
    [InlineData(typeof(CartItem), "carrito_items")]
    [InlineData(typeof(Sale), "venta")]
    [InlineData(typeof(SaleDetail), "detalle_venta")]
    [InlineData(typeof(CashMovement), "movimiento")]
    public void Entidades_UsanLosNombresDeTablaDelEsquema(Type type, string expectedTable)
    {
        var entity = Context.Model.FindEntityType(type);
        Assert.NotNull(entity);
        Assert.Equal(expectedTable, entity!.GetTableName());
    }

    [Theory]
    [InlineData(typeof(Cliente), nameof(Cliente.Id), "id_cliente")]
    [InlineData(typeof(Cliente), nameof(Cliente.PasswordHash), "contrasena_hash")]
    [InlineData(typeof(Product), nameof(Product.ID), "id_producto")]
    [InlineData(typeof(Product), nameof(Product.Price), "precio_ventas")]
    [InlineData(typeof(Order), nameof(Order.Id), "id_pedido")]
    [InlineData(typeof(Order), nameof(Order.ClienteId), "id_cliente")]
    [InlineData(typeof(Order), nameof(Order.Status), "estado")]
    [InlineData(typeof(CartItem), nameof(CartItem.ProductId), "id_producto")]
    [InlineData(typeof(CartItem), nameof(CartItem.Quantity), "cantidad")]
    [InlineData(typeof(Sale), nameof(Sale.Id), "id_venta")]
    [InlineData(typeof(Sale), nameof(Sale.UsuarioId), "id_usuario")]
    [InlineData(typeof(SaleDetail), nameof(SaleDetail.SaleId), "id_venta")]
    [InlineData(typeof(SaleDetail), nameof(SaleDetail.UnitCost), "costo_unitario")]
    [InlineData(typeof(CashMovement), nameof(CashMovement.Id), "id_movimiento")]
    [InlineData(typeof(CashMovement), nameof(CashMovement.Amount), "monto")]
    public void Propiedades_UsanLosNombresDeColumnaDelEsquema(Type type, string property, string expectedColumn)
    {
        var entity = Context.Model.FindEntityType(type);
        Assert.NotNull(entity);
        var tableName = entity!.GetTableName();
        Assert.NotNull(tableName);
        var storeObject = StoreObjectIdentifier.Table(tableName!, entity.GetSchema());
        var propertyMetadata = entity.FindProperty(property);

        Assert.NotNull(propertyMetadata);
        Assert.Equal(expectedColumn, propertyMetadata!.GetColumnName(storeObject));
    }

    [Fact]
    public void LineaDePedido_TieneLlaveForaneaHaciaPedido()
    {
        var entity = Context.Model.FindEntityType(typeof(CartItem));
        Assert.NotNull(entity);

        var orderRelationships = entity!.GetForeignKeys()
            .Where(fk => fk.PrincipalEntityType.ClrType == typeof(Order))
            .ToList();
        Assert.Single(orderRelationships);
        var orderRelationship = orderRelationships[0];

        Assert.Contains(orderRelationship.Properties, property => property.Name == "OrderId");
        Assert.Equal("id_pedido", orderRelationship.Properties.Single().GetColumnName(
            StoreObjectIdentifier.Table("carrito_items", null)));
    }
}
