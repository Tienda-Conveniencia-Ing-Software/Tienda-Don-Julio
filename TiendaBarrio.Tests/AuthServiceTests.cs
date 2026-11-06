using Microsoft.EntityFrameworkCore;
using TiendaBarrio.Core.Models;
using TiendaBarrio.Core.Services;
using TiendaBarrio.Persistence.Data;

namespace TiendaBarrio.Tests;

public class AuthServiceTests
{
    private AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task LoginAsync_ConCredencialesValidas_DevuelveElCliente()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var cliente = new Cliente(1, "Admin", "123", "300", "admin@test.com", BCrypt.Net.BCrypt.HashPassword("admin123"), "Trabajador");
        context.Clientes.Add(cliente);
        await context.SaveChangesAsync();

        var authService = new AuthService(context);

        // Act
        var result = await authService.LoginAsync("admin@test.com", "admin123");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("admin@test.com", result.Email);
    }

    [Fact]
    public async Task LoginAsync_ConPasswordIncorrecta_DevuelveNull()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var cliente = new Cliente(1, "Admin", "123", "300", "admin@test.com", BCrypt.Net.BCrypt.HashPassword("admin123"), "Trabajador");
        context.Clientes.Add(cliente);
        await context.SaveChangesAsync();

        var authService = new AuthService(context);

        // Act
        var result = await authService.LoginAsync("admin@test.com", "incorrecta");

        // Assert
        Assert.Null(result);
    }
}
