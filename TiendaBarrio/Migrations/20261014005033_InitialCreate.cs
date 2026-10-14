using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TiendaBarrio.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Cedula = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Telefono = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Movimientos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<double>(type: "numeric(10,2)", nullable: false),
                    Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movimientos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pedidos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Total = table.Column<double>(type: "double precision", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pedidos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Productos",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Price = table.Column<double>(type: "numeric(10,2)", nullable: false),
                    PurchasePrice = table.Column<double>(type: "numeric(10,2)", nullable: false),
                    Stock = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productos", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NombreUsuario = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ContraseñaHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Rol = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ventas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Total = table.Column<double>(type: "numeric(10,2)", nullable: false),
                    TotalCost = table.Column<double>(type: "numeric(10,2)", nullable: false),
                    Profit = table.Column<double>(type: "numeric(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ventas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Carrito_Items",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    OrderId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carrito_Items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Carrito_Items_Pedidos_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Pedidos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Carrito_Items_Productos_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Productos",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Detalle_Venta",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    ProductName = table.Column<string>(type: "text", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    UnitPrice = table.Column<double>(type: "numeric(10,2)", nullable: false),
                    UnitCost = table.Column<double>(type: "numeric(10,2)", nullable: false),
                    SaleId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Detalle_Venta", x => new { x.ProductId, x.ProductName });
                    table.ForeignKey(
                        name: "FK_Detalle_Venta_Ventas_SaleId",
                        column: x => x.SaleId,
                        principalTable: "Ventas",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Clientes",
                columns: new[] { "Id", "Cedula", "Email", "Nombre", "PasswordHash", "Role", "Telefono" },
                values: new object[,]
                {
                    { 1, "123456789", "admin@donjulio.com", "Administrador Don Julio", "$2b$11$rLjyHkVUytKYZbxncw/63eKyr2MuYGtdDzIT6gY/0JN3RK3qZPVwe", "Trabajador", "3001234567" },
                    { 2, "12345678", "juan@test.com", "Juan Perez", "$2b$11$calnlnE2nMpqpTyMN89Ul.G3YX2fnqG1YsL3pc/KflTvxRKcmW.1e", "Cliente", "3001234567" },
                    { 3, "87654321", "maria@test.com", "Maria Lopez", "$2b$11$calnlnE2nMpqpTyMN89Ul.G3YX2fnqG1YsL3pc/KflTvxRKcmW.1e", "Cliente", "3007654321" },
                    { 4, "11223344", "carlos@test.com", "Carlos Ruiz", "$2b$11$calnlnE2nMpqpTyMN89Ul.G3YX2fnqG1YsL3pc/KflTvxRKcmW.1e", "Trabajador", "3001122334" },
                    { 5, "44332211", "ana@test.com", "Ana Torres", "$2b$11$calnlnE2nMpqpTyMN89Ul.G3YX2fnqG1YsL3pc/KflTvxRKcmW.1e", "Cliente", "3004433221" },
                    { 6, "55667788", "pedro@test.com", "Pedro Gomez", "$2b$11$calnlnE2nMpqpTyMN89Ul.G3YX2fnqG1YsL3pc/KflTvxRKcmW.1e", "Cliente", "3005566778" }
                });

            migrationBuilder.InsertData(
                table: "Productos",
                columns: new[] { "ID", "Name", "Price", "PurchasePrice", "Stock" },
                values: new object[,]
                {
                    { 1, "Arroz", 2500.0, 1800.0, 100 },
                    { 2, "Leche", 3500.0, 2800.0, 50 },
                    { 3, "Pan", 2000.0, 1500.0, 80 },
                    { 4, "Huevos", 500.0, 350.0, 200 },
                    { 5, "Café", 8000.0, 6000.0, 30 },
                    { 6, "Azúcar", 3000.0, 2200.0, 60 },
                    { 7, "Sal", 1500.0, 1000.0, 100 },
                    { 8, "Aceite", 9000.0, 7000.0, 40 }
                });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "ContraseñaHash", "Email", "FechaCreacion", "NombreUsuario", "Rol" },
                values: new object[,]
                {
                    { 1, "admin123", "admin@tienda.com", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin", "Admin" },
                    { 2, "vendedor123", "vendedor@tienda.com", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "vendedor", "Vendedor" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Carrito_Items_OrderId",
                table: "Carrito_Items",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Carrito_Items_ProductId",
                table: "Carrito_Items",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Detalle_Venta_SaleId",
                table: "Detalle_Venta",
                column: "SaleId");

            migrationBuilder.Sql(@"SELECT setval(pg_get_serial_sequence('""Productos""', 'ID'), (SELECT MAX(""ID"") FROM ""Productos""));");
            migrationBuilder.Sql(@"SELECT setval(pg_get_serial_sequence('""Clientes""', 'Id'), (SELECT MAX(""Id"") FROM ""Clientes""));");
            migrationBuilder.Sql(@"SELECT setval(pg_get_serial_sequence('""Usuarios""', 'Id'), (SELECT MAX(""Id"") FROM ""Usuarios""));");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Carrito_Items");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "Detalle_Venta");

            migrationBuilder.DropTable(
                name: "Movimientos");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Pedidos");

            migrationBuilder.DropTable(
                name: "Productos");

            migrationBuilder.DropTable(
                name: "Ventas");
        }
    }
}
