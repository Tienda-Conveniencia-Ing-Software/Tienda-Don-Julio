using TiendaBarrio.Core.Models;

namespace TiendaBarrio.Tests;

public class OrderTests
{
    [Fact]
    public void NuevoPedido_EmpiezaPendienteYSinItems()
    {
        var order = new Order();

        Assert.Equal(OrderStatus.Pendiente, order.Status);
        Assert.Empty(order.Items);
    }

    [Fact]
    public void Constructor_ConItems_CalculaElTotal()
    {
        var arroz = new Product(1, "Arroz", 2500, 1800, 100);
        var leche = new Product(2, "Leche", 3500, 2800, 50);
        var items = new List<CartItem> { new(arroz, 2), new(leche, 1) };   // 5000 + 3500

        var order = new Order(10, items, clienteId: 2);

        Assert.Equal(10, order.Id);
        Assert.Equal(2, order.ClienteId);
        Assert.Equal(8500, order.Total);
        Assert.Equal(OrderStatus.Pendiente, order.Status);
    }

    [Theory]
    [InlineData(OrderStatus.EnProceso)]
    [InlineData(OrderStatus.Completado)]
    [InlineData(OrderStatus.Cancelado)]
    public void AdvanceStatus_CambiaElEstado(OrderStatus nuevo)
    {
        var order = new Order();

        order.AdvanceStatus(nuevo);

        Assert.Equal(nuevo, order.Status);
    }
}

public class CartItemTests
{
    [Fact]
    public void Constructor_CopiaElPrecioDelProducto()
    {
        var arroz = new Product(1, "Arroz", 2500, 1800, 100);

        var item = new CartItem(arroz, 3);

        Assert.Equal(1, item.ProductId);
        Assert.Equal(2500, item.UnitPrice);
        Assert.Equal(7500, item.Subtotal);
    }

    [Fact]
    public void Subtotal_UsaElPrecioGuardado_AunqueElProductoCambieDePrecio()
    {
        var arroz = new Product(1, "Arroz", 2500, 1800, 100);
        var item = new CartItem(arroz, 2);

        arroz.Price = 9999;   // el precio sube después de crear el pedido

        Assert.Equal(5000, item.Subtotal);
    }

    [Fact]
    public void Subtotal_SinPrecioGuardado_UsaElPrecioDelProducto()
    {
        var leche = new Product(2, "Leche", 3500, 2800, 50);
        var item = new CartItem { Product = leche, Quantity = 2 };   // UnitPrice = 0

        Assert.Equal(7000, item.Subtotal);
    }

    [Fact]
    public void SetQuantity_ConCantidadPositiva_LaActualiza()
    {
        var item = new CartItem(new Product(1, "Pan", 2000, 1500, 80), 1);

        item.SetQuantity(5);

        Assert.Equal(5, item.Quantity);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-3)]
    public void SetQuantity_ConCantidadCeroONegativa_LaIgnora(int cantidad)
    {
        var item = new CartItem(new Product(1, "Pan", 2000, 1500, 80), 4);

        item.SetQuantity(cantidad);

        Assert.Equal(4, item.Quantity);
    }
}
