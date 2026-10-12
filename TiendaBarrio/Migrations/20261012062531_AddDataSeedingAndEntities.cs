using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TiendaBarrio.Migrations
{
    /// <inheritdoc />
    public partial class AddDataSeedingAndEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Clientes",
                columns: new[] { "Id", "Cedula", "Email", "Nombre", "PasswordHash", "Role", "Telefono" },
                values: new object[] { 1, "123456789", "admin@donjulio.com", "Administrador Don Julio", "$2a$11$s18O2WqXF1zUj9kZq3B3ee5XpX3aO1NfL9/aK0vA7Zq0gU3g0Oa8i", "Trabajador", "3001234567" });

            migrationBuilder.InsertData(
                table: "Productos",
                columns: new[] { "ID", "Name", "Price", "PurchasePrice", "Stock" },
                values: new object[,]
                {
                    { 1, "Arroz 1kg", 4500.0, 3500.0, 50 },
                    { 2, "Leche 1L", 3800.0, 3000.0, 30 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Clientes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "ID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "ID",
                keyValue: 2);
        }
    }
}
