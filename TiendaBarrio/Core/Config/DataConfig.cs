namespace TiendaBarrio.Core.Config;

public static class DataConfig
{
    // Carpeta base donde se guardan todos los datos
    private static readonly string DataFolder = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Data");

    // Rutas de archivos
    public static string ProductosFile => Path.Combine(DataFolder, "productos.txt");
    public static string VentasFile => Path.Combine(DataFolder, "ventas.txt");
    public static string PedidosFile => Path.Combine(DataFolder, "pedidos.txt");
    public static string ClientesFile => Path.Combine(DataFolder, "clientes.txt");
    public static string MovimientosFile => Path.Combine(DataFolder, "movimientos.txt");

    // Asegura que la carpeta exista
    public static void EnsureDataFolderExists()
    {
        if (!Directory.Exists(DataFolder))
        {
            Directory.CreateDirectory(DataFolder);
        }
    }
}
