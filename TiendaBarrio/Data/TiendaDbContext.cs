namespace TiendaBarrio.Data;

using Microsoft.EntityFrameworkCore;
using TiendaBarrio.Core.Models;

public class TiendaDbContext : DbContext
{
    // Constructor por defecto (para instanciación manual)
    public TiendaDbContext() { }

    // Constructor para Inyección de Dependencias (requerido por Web API y Blazor)
    public TiendaDbContext(DbContextOptions<TiendaDbContext> options) : base(options) { }

    // DbSets (cada uno representa una tabla)
    public DbSet<Product> Productos { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
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
            var password = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "neji1514j*";

            var connectionString = $"Host={host};Port={port};Database={database};Username={user};Password={password}";

            optionsBuilder.UseNpgsql(connectionString);
        }
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

    // Seed: Usuarios del sistema
    modelBuilder.Entity<Usuario>().HasData(
        new Usuario
        {
            Id = 1,
            NombreUsuario = "admin",
            Email = "admin@tienda.com",
            ContraseñaHash = "admin123",
            Rol = "Admin",
            FechaCreacion = new DateTime(2026, 1, 1)
        },
        new Usuario
        {
            Id = 2,
            NombreUsuario = "vendedor",
            Email = "vendedor@tienda.com",
            ContraseñaHash = "vendedor123",
            Rol = "Vendedor",
            FechaCreacion = new DateTime(2026, 1, 1)
        }
    );

    // Seed: Clientes de prueba
    modelBuilder.Entity<Cliente>().HasData(
        new Cliente { Id = 1, Nombre = "Administrador Don Julio", Cedula = "123456789", Telefono = "3001234567", Email = "admin@donjulio.com", PasswordHash = "hash_admin", Role = "Trabajador" },
        new Cliente { Id = 2, Nombre = "Juan Perez", Cedula = "12345678", Telefono = "3001234567", Email = "juan@test.com", PasswordHash = "hash_juan", Role = "Cliente" },
        new Cliente { Id = 3, Nombre = "Maria Lopez", Cedula = "87654321", Telefono = "3007654321", Email = "maria@test.com", PasswordHash = "hash_maria", Role = "Cliente" },
        new Cliente { Id = 4, Nombre = "Carlos Ruiz", Cedula = "11223344", Telefono = "3001122334", Email = "carlos@test.com", PasswordHash = "hash_carlos", Role = "Trabajador" },
        new Cliente { Id = 5, Nombre = "Ana Torres", Cedula = "44332211", Telefono = "3004433221", Email = "ana@test.com", PasswordHash = "hash_ana", Role = "Cliente" },
        new Cliente { Id = 6, Nombre = "Pedro Gomez", Cedula = "55667788", Telefono = "3005566778", Email = "pedro@test.com", PasswordHash = "hash_pedro", Role = "Cliente" }
    );

    // Seed: Catálogo base de productos
    modelBuilder.Entity<Product>().HasData(
        new Product { ID = 1, Name = "Arroz", Price = 2500, PurchasePrice = 1800, Stock = 100 },
        new Product { ID = 2, Name = "Leche", Price = 3500, PurchasePrice = 2800, Stock = 50 },
        new Product { ID = 3, Name = "Pan", Price = 2000, PurchasePrice = 1500, Stock = 80 },
        new Product { ID = 4, Name = "Huevos", Price = 500, PurchasePrice = 350, Stock = 200 },
        new Product { ID = 5, Name = "Café", Price = 8000, PurchasePrice = 6000, Stock = 30 },
        new Product { ID = 6, Name = "Azúcar", Price = 3000, PurchasePrice = 2200, Stock = 60 },
        new Product { ID = 7, Name = "Sal", Price = 1500, PurchasePrice = 1000, Stock = 100 },
        new Product { ID = 8, Name = "Aceite", Price = 9000, PurchasePrice = 7000, Stock = 40 }
    );
    }
}   
