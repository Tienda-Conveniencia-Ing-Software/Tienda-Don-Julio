namespace TiendaBarrio;

using TiendaBarrio.Core.Models;
using TiendaBarrio.UI;
class Program
{
    static void Main()
    {
        MainMenu menu = new MainMenu(new List<Product>());
        menu.Start();
    }
}