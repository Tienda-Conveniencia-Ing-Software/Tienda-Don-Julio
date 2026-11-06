using Moq;
using TiendaBarrio.Core.Models;
using TiendaBarrio.Core.Services;
using TiendaBarrio.Persistence.Interfaces;

namespace TiendaBarrio.Tests;

public class InventoryServiceTests
{
    private readonly Mock<IProductRepository> _productRepoMock;
    private readonly Mock<FinanceService> _financeServiceMock;
    private readonly InventoryService _inventoryService;

    public InventoryServiceTests()
    {
        _productRepoMock = new Mock<IProductRepository>();
        _financeServiceMock = new Mock<FinanceService>(MockBehavior.Loose, new Mock<ICashRepository>().Object);
        _inventoryService = new InventoryService(_productRepoMock.Object, _financeServiceMock.Object);
    }

    [Fact]
    public async Task RegisterProductAsync_ConDatosValidos_AgregaElProducto()
    {
        // Arrange
        var products = new List<Product>();
        var admin = new Cliente { Role = "Trabajador" };

        // Act
        var result = await _inventoryService.RegisterProductAsync(admin, products, "Arroz", 2500, 1800, 100);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Arroz", result.Name);
        Assert.Equal(100, result.Stock);
        Assert.Single(products);
    }

    [Fact]
    public async Task IncreaseStockAsync_ConCantidadValida_AumentaElStock()
    {
        // Arrange
        var product = new Product(1, "Arroz", 2500, 1800, 10);
        var products = new List<Product> { product };
        var admin = new Cliente { Role = "Trabajador" };

        // Act
        var result = await _inventoryService.IncreaseStockAsync(admin, products, 1, 5);

        // Assert
        Assert.True(result);
        Assert.Equal(15, product.Stock);
    }

    [Fact]
    public async Task DecreaseStockAsync_ConCantidadMayorAlStock_NoCambiaElStock()
    {
        // Arrange
        var product = new Product(1, "Arroz", 2500, 1800, 10);
        var products = new List<Product> { product };
        var admin = new Cliente { Role = "Trabajador" };

        // Act
        var result = await _inventoryService.DecreaseStockAsync(admin, products, 1, 20);

        // Assert
        Assert.False(result);
        Assert.Equal(10, product.Stock);
    }
}
