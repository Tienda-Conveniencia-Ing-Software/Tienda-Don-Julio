using DotNetEnv;

namespace TiendaBarrio;

using Microsoft.Extensions.DependencyInjection;
using TiendaBarrio.Core.Config;
using TiendaBarrio.Data;
using TiendaBarrio.UI;

class Program
{
    static void Main()
    {
        // Cargar variables de entorno desde .env (antes de crear el DbContext).
        // Se busca en la carpeta del proyecto para que funcione sin importar
        // desde dónde se ejecute el comando.
        string envPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".env");
        if (File.Exists(envPath))
        {
            Env.Load(envPath);
        }

        var services = new ServiceCollection();

        // Base de datos (usa la configuración por defecto de TiendaDbContext)
        services.AddDbContext<TiendaDbContext>();

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
        scope.ServiceProvider.GetRequiredService<MainMenu>().Start();
    }
}