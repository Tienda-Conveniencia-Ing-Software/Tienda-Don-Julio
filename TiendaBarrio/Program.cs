using DotNetEnv;

namespace TiendaBarrio;

using Microsoft.Extensions.DependencyInjection;
using TiendaBarrio.Core.Config;
using TiendaBarrio.Persistence.Data;
using TiendaBarrio.UI;

class Program
{
    static async Task Main()
    {
        // Cargar variables de entorno desde .env (antes de crear el DbContext).
        string envPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".env");
        if (File.Exists(envPath))
        {
            Env.Load(envPath);
        }

        var services = new ServiceCollection();

        // Base de datos (usa la configuración por defecto de AppDbContext)
        services.AddDbContext<AppDbContext>();

        // Repositorios y servicios del Core
        services.AddTiendaCore();

        // Menús de consola
        services.AddScoped<ShowProducts>();
        services.AddScoped<CartMenu>();
        services.AddScoped<SalesMenu>();
        services.AddScoped<InventoryMenu>();
        services.AddScoped<FinanceMenu>();
        services.AddScoped<MainMenu>();

        using var provider = services.BuildServiceProvider();

        // Un solo scope para toda la sesión: el carrito y FinanceService se comparten
        using var scope = provider.CreateScope();
        await scope.ServiceProvider.GetRequiredService<MainMenu>().StartAsync();
    }
}