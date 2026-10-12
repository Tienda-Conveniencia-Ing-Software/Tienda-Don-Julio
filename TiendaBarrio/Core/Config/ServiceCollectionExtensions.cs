namespace TiendaBarrio.Core.Config;

using Microsoft.Extensions.DependencyInjection;
using TiendaBarrio.Core.Services;
using TiendaBarrio.Persistence;
using TiendaBarrio.Persistence.Interfaces;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registra repositorios y servicios del Core. El DbContext lo registra
    /// cada aplicacion (consola, API, Web) porque su configuracion cambia.
    /// </summary>
    public static IServiceCollection AddTiendaCore(this IServiceCollection services)
    {
        // Repositorios
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<ICashRepository, CashRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ISaleRepository, SaleRepository>();

        // Servicios
        services.AddScoped<AuthService>();
        services.AddScoped<FinanceService>();
        services.AddScoped<InventoryService>();
        services.AddScoped<CartService>();
        services.AddScoped<OrderService>();
        services.AddScoped<SaleService>();

        return services;
    }
}
