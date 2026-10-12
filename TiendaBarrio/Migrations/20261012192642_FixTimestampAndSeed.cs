using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TiendaBarrio.Migrations
{
    /// <inheritdoc />
    public partial class FixTimestampAndSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "Ventas",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaCreacion",
                table: "Usuarios",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "Movimientos",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.InsertData(
                table: "Clientes",
                columns: new[] { "Id", "Cedula", "Email", "Nombre", "PasswordHash", "Role", "Telefono" },
                values: new object[,]
                {
                    { 1, "123456789", "admin@donjulio.com", "Administrador Don Julio", "hash_admin", "Trabajador", "3001234567" },
                    { 2, "12345678", "juan@test.com", "Juan Perez", "hash_juan", "Cliente", "3001234567" },
                    { 3, "87654321", "maria@test.com", "Maria Lopez", "hash_maria", "Cliente", "3007654321" },
                    { 4, "11223344", "carlos@test.com", "Carlos Ruiz", "hash_carlos", "Trabajador", "3001122334" },
                    { 5, "44332211", "ana@test.com", "Ana Torres", "hash_ana", "Cliente", "3004433221" },
                    { 6, "55667788", "pedro@test.com", "Pedro Gomez", "hash_pedro", "Cliente", "3005566778" }
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Clientes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Clientes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Clientes",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Clientes",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Clientes",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Clientes",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "ID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "ID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "ID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "ID",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "ID",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "ID",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "ID",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "ID",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "Ventas",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaCreacion",
                table: "Usuarios",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "Movimientos",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");
        }
    }
}
