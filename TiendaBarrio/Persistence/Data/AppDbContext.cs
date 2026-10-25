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
            var password = Environment.GetEnvironmentVariable("DB_PASSWORD");
            if (string.IsNullOrWhiteSpace(password))
            {
                password = "postgres"; // Contraseña por defecto para local
            }
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
        entity.ToTable("Clientes"); // ⬅️ Cambiado a plural y mayúscula
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).HasColumnName("Id"); // ⬅️ EF Core usará "Id" por defecto
        entity.Property(e => e.Nombre).HasColumnName("Nombre");
        entity.Property(e => e.Cedula).HasColumnName("Cedula");
        entity.Property(e => e.Telefono).HasColumnName("Telefono");
        entity.Property(e => e.Email).HasColumnName("Email");
        entity.Property(e => e.PasswordHash).HasColumnName("PasswordHash");
        entity.Property(e => e.Role).HasColumnName("Role");
    });

    // ✅ Mapeo: Producto
    modelBuilder.Entity<Product>(entity =>
    {
        entity.ToTable("Productos"); // ⬅️ Cambiado a plural y mayúscula
        entity.HasKey(e => e.ID);
        entity.Property(e => e.ID).HasColumnName("ID");
        entity.Property(e => e.Name).HasColumnName("Name");
        entity.Property(e => e.Price).HasColumnName("Price");
        entity.Property(e => e.PurchasePrice).HasColumnName("PurchasePrice");
        entity.Property(e => e.Stock).HasColumnName("Stock");
    });

    // ✅ Mapeo: Pedido (Order)
    modelBuilder.Entity<Order>(entity =>
    {
        entity.ToTable("Pedidos"); // ⬅️ Cambiado
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).HasColumnName("Id");
        entity.Property(e => e.ClienteId).HasColumnName("ClienteId");
        entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt");
        entity.Property(e => e.Status).HasColumnName("Status");
        entity.Property(e => e.Total).HasColumnName("Total");
    });

    // ✅ Mapeo: CarritoItem
    modelBuilder.Entity<CartItem>(entity =>
    {
        entity.ToTable("Carrito_Items"); // ⬅️ Cambiado
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).HasColumnName("Id");
        //entity.Property(e => e.OrderId).HasColumnName("OrderId");
        entity.Property(e => e.ProductId).HasColumnName("ProductId");
        entity.Property(e => e.Quantity).HasColumnName("Quantity");
        entity.Property(e => e.UnitPrice).HasColumnName("UnitPrice");
    });

    // ✅ Mapeo: Venta (Sale)
    modelBuilder.Entity<Sale>(entity =>
    {
        entity.ToTable("Ventas"); // ⬅️ Cambiado
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).HasColumnName("Id");
        entity.Property(e => e.Date).HasColumnName("Date");
        entity.Property(e => e.Total).HasColumnName("Total");
        entity.Property(e => e.TotalCost).HasColumnName("TotalCost");
        entity.Property(e => e.Profit).HasColumnName("Profit");
    });

    // ✅ Mapeo: DetalleVenta (SaleDetail)
    modelBuilder.Entity<SaleDetail>(entity =>
    {
        entity.ToTable("Detalle_Venta"); // ⬅️ Cambiado
        entity.HasKey(e => e.ProductId);
        entity.Property(e => e.ProductId).HasColumnName("ProductId");
        entity.Property(e => e.ProductName).HasColumnName("ProductName");
        entity.Property(e => e.Quantity).HasColumnName("Quantity");
        entity.Property(e => e.UnitPrice).HasColumnName("UnitPrice");
        entity.Property(e => e.UnitCost).HasColumnName("UnitCost");
    });

        // ✅ Mapeo: Movimiento (CashMovement)
    modelBuilder.Entity<CashMovement>(entity =>
    {
        entity.ToTable("Movimientos"); // ⬅️ Cambiado
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).HasColumnName("Id");
        entity.Property(e => e.Type).HasColumnName("Type").HasConversion<string>();
        entity.Property(e => e.Description).HasColumnName("Description");
        entity.Property(e => e.Amount).HasColumnName("Amount");
        entity.Property(e => e.Date).HasColumnName("Date");
    });
    } 
} 


   
