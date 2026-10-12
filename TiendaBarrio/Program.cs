using DotNetEnv;

namespace TiendaBarrio;

using TiendaBarrio.Core.Models;
using TiendaBarrio.UI;

class Program
{
    static void Main()
    {
        // ✅ Cargar variables de entorno desde .env
        Env.Load();

        MainMenu menu = new MainMenu();
        menu.Start();
    }
}