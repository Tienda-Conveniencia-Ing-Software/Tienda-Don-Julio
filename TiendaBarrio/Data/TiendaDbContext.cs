namespace TiendaBarrio.Data;

using Microsoft.EntityFrameworkCore;
using TiendaBarrio.Core.Models;

public class TiendaDbContext : DbContext
{
    // DbSets (cada uno representa una tabla)
    public DbSet<Product> Productos { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Sale> Ventas { get; set; }
    public DbSet<SaleDetail> DetalleVentas { get; set; }
    public DbSet<Order> Pedidos { get; set; }
    public DbSet<CartItem> CarritoItems { get; set; }
    public DbSet<CashMovement> Movimientos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        // Cadena de conexión con variables de entorno (sin hardcodear)
        var host = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
        var port = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
        var database = Environment.GetEnvironmentVariable("DB_NAME") ?? "BD_Tienda";
        var user = Environment.GetEnvironmentVariable("DB_USER") ?? "postgres";
        var password = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "postgres";

        var connectionString = $"Host={host};Port={port};Database={database};Username={user};Password={password}";

        options.UseNpgsql(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Mapeo: Productos
        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Productos");
            entity.HasKey(e => e.ID);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(10,2)");
            entity.Property(e => e.PurchasePrice).HasColumnType("decimal(10,2)");
        });

        // Mapeo: Clientes
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.ToTable("Clientes");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Cedula).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Telefono).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(100);
        });

        // Mapeo: Usuarios
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("Usuarios");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.NombreUsuario).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ContraseñaHash).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Rol).IsRequired().HasMaxLength(20);
        });

        // Mapeo: Ventas
        modelBuilder.Entity<Sale>(entity =>
        {
            entity.ToTable("Ventas");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Total).HasColumnType("decimal(10,2)");
            entity.Property(e => e.TotalCost).HasColumnType("decimal(10,2)");
            entity.Property(e => e.Profit).HasColumnType("decimal(10,2)");
        });

        // Mapeo: Detalle de Ventas
        modelBuilder.Entity<SaleDetail>(entity =>
        {
            entity.ToTable("Detalle_Venta");
            entity.HasKey(e => new { e.ProductId, e.ProductName });
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(10,2)");
            entity.Property(e => e.UnitCost).HasColumnType("decimal(10,2)");
        });

        // Mapeo: Pedidos
        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("Pedidos");
            entity.HasKey(e => e.Id);
            entity.HasMany(e => e.Items)
                  .WithOne()
                  .HasForeignKey("OrderId")
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Mapeo: Carrito de Compras
        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.ToTable("Carrito_Items");
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Product)
                  .WithMany()
                  .HasForeignKey(e => e.ProductId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Mapeo: Movimientos (CashMovement)
        modelBuilder.Entity<CashMovement>(entity =>
        {
            entity.ToTable("Movimientos");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Type).IsRequired();
            entity.Property(e => e.Amount).HasColumnType("decimal(10,2)");
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.Date).IsRequired();
        });
    }
}
