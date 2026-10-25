namespace TiendaBarrio.Persistence.Data;

using Microsoft.EntityFrameworkCore;
using TiendaBarrio.Core.Models;

/// <summary>
/// EF Core solo MAPEA las entidades a las tablas creadas por los scripts
/// database/init/01-schema.sql y 02-seed.sql. Ya no crea tablas, no usa
/// migraciones ni inserta datos (HasData): todo eso vive en el script SQL.
/// Si cambias una tabla o columna, cambia el script Y el mapeo de aquí.
/// </summary>
public class AppDbContext : DbContext
{
    // Constructor por defecto (para instanciación manual)
    public AppDbContext() { }

    // Constructor para Inyección de Dependencias (requerido por Web API y Blazor)
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // DbSets (cada uno representa una tabla)
    public DbSet<Product> Productos { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Sale> Ventas { get; set; }
    public DbSet<SaleDetail> DetalleVentas { get; set; }
    public DbSet<Order> Pedidos { get; set; }
    public DbSet<CartItem> CarritoItems { get; set; }
    public DbSet<CashMovement> Movimientos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Solo configura si no ha sido configurado previamente por la Inyección de Dependencias
        if (!optionsBuilder.IsConfigured)
        {
            var host = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
            var port = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
            var database = Environment.GetEnvironmentVariable("DB_NAME") ?? "BD_Tienda";
            var user = Environment.GetEnvironmentVariable("DB_USER") ?? "postgres";
            var password = Environment.GetEnvironmentVariable("DB_PASSWORD")
                ?? throw new InvalidOperationException("La variable de entorno DB_PASSWORD no está configurada.");

            var connectionString = $"Host={host};Port={port};Database={database};Username={user};Password={password}";

            optionsBuilder.UseNpgsql(connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    // ✅ Mapeo: Cliente
    modelBuilder.Entity<Cliente>(entity =>
    {
        entity.ToTable("cliente");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).HasColumnName("id_cliente");
        entity.Property(e => e.Nombre).HasColumnName("nombre_cliente").HasMaxLength(100).IsRequired();
        entity.Property(e => e.Cedula).HasColumnName("cedula").HasMaxLength(20).IsRequired();
        entity.Property(e => e.Telefono).HasColumnName("telefono").HasMaxLength(20);
        entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(100).IsRequired();
        entity.Property(e => e.PasswordHash).HasColumnName("contrasena_hash").HasMaxLength(255).IsRequired();
        entity.Property(e => e.Role).HasColumnName("rol").HasMaxLength(20).IsRequired();
        // Nota: fecha_creacion no está en la clase Cliente, si la agregas, mapeala aquí.
    });

    // ✅ Mapeo: Producto
    modelBuilder.Entity<Product>(entity =>
    {
        entity.ToTable("producto");
        entity.HasKey(e => e.ID);
        entity.Property(e => e.ID).HasColumnName("id_producto");
        entity.Property(e => e.Name).HasColumnName("nombre_producto").HasMaxLength(100).IsRequired();
        entity.Property(e => e.Price).HasColumnName("precio_ventas").HasColumnType("decimal(10,2)");
        entity.Property(e => e.PurchasePrice).HasColumnName("precio_compra").HasColumnType("decimal(10,2)");
        entity.Property(e => e.Stock).HasColumnName("stock");
    });

    // ✅ Mapeo: Pedido (Order)
    modelBuilder.Entity<Order>(entity =>
    {
        entity.ToTable("pedido");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).HasColumnName("id_pedido");
        entity.Property(e => e.ClienteId).HasColumnName("id_cliente");
        entity.Property(e => e.CreatedAt).HasColumnName("fecha");
        entity.Property(e => e.Status).HasColumnName("estado");
        entity.Property(e => e.Total).HasColumnName("total").HasColumnType("decimal(10,2)");
        // Se asume que la relación con Items se maneja a través de la FK en CarritoItem.
    });

    // ✅ Mapeo: CarritoItem (Detalle del Pedido)
    modelBuilder.Entity<CartItem>(entity =>
    {
        entity.ToTable("carrito_items");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).HasColumnName("id_carrito_items");
        entity.Property(e => e.OrderId).HasColumnName("id_pedido");
        entity.Property(e => e.ProductId).HasColumnName("id_producto");
        entity.Property(e => e.Quantity).HasColumnName("cantidad");
        entity.Property(e => e.UnitPrice).HasColumnName("precio_unitario").HasColumnType("decimal(10,2)");
    });

    // ✅ Mapeo: Venta (Sale)
    modelBuilder.Entity<Sale>(entity =>
    {
        entity.ToTable("venta");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).HasColumnName("id_venta");
        // entity.Property(e => e.ClienteId).HasColumnName("id_cliente"); // Si Sale tiene esta prop
        entity.Property(e => e.Date).HasColumnName("fecha_venta");
        entity.Property(e => e.Total).HasColumnName("total").HasColumnType("decimal(10,2)");
        entity.Property(e => e.TotalCost).HasColumnName("total_costo").HasColumnType("decimal(10,2)");
        entity.Property(e => e.Profit).HasColumnName("ganancia").HasColumnType("decimal(10,2)");
    });

    // ✅ Mapeo: DetalleVenta (SaleDetail)
    modelBuilder.Entity<SaleDetail>(entity =>
    {
        entity.ToTable("detalle_venta");
        entity.HasKey(e => e.ProductId); // Ojo: Si SaleDetail tiene un ID propio, cambia esto por ese ID.
        entity.Property(e => e.ProductId).HasColumnName("id_producto");
        entity.Property(e => e.ProductName).HasColumnName("nombre_producto").HasMaxLength(100);
        entity.Property(e => e.Quantity).HasColumnName("cantidad");
        entity.Property(e => e.UnitPrice).HasColumnName("precio_unitario").HasColumnType("decimal(10,2)");
        entity.Property(e => e.UnitCost).HasColumnName("costo_unitario").HasColumnType("decimal(10,2)");
    });

    // ✅ Mapeo: Movimiento (CashMovement)
    modelBuilder.Entity<CashMovement>(entity =>
    {
        entity.ToTable("movimiento");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).HasColumnName("id_movimiento");
        entity.Property(e => e.Type).HasColumnName("tipo").HasConversion<string>();
        entity.Property(e => e.Description).HasColumnName("descripcion").HasMaxLength(200);
        entity.Property(e => e.Amount).HasColumnName("monto").HasColumnType("decimal(10,2)");
        entity.Property(e => e.Date).HasColumnName("fecha_movimiento");
        // Nota: Si CashMovement tiene UsuarioId, mapealo aquí.
    });
}

    
