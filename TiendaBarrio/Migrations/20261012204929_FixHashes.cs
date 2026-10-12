using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TiendaBarrio.Migrations
{
    /// <inheritdoc />
    public partial class FixHashes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$12$v3OitJliBa8wBq8R5u6X0uyAbBJLBxZi46Qy8DV1.G3AUnMUQrlBK");

            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$12$IVyD6Zrqx/56rH0Z26ZUW.AJTTY4The7jE.kP34Qp6dVoUONMiKS2");

            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "Id",
                keyValue: 3,
                column: "PasswordHash",
                value: "$2a$12$2an1Qf9ahpOJ74OGBM8ev.OosG3IyRC1gxE5DrfIXMv9nSar8R1ES");

            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "Id",
                keyValue: 4,
                column: "PasswordHash",
                value: "$2a$12$xrzLQfC9JD/GxRJlWHy0zuPXibr4FxnPUZ3lkrL8dn8KT6IICdRRS");

            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "Id",
                keyValue: 5,
                column: "PasswordHash",
                value: "$2a$12$SSpq.ci2nf4CY0DpJAM8wuWhy8NRcmFShe9f3ARznPTiAPBfl.2IS");

            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "Id",
                keyValue: 6,
                column: "PasswordHash",
                value: "$2a$12$8/.m406KNTdh2g3rpfWrQO2apkhQq7NijEg7NnPraRfdCCysCL6h.");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "ContraseñaHash",
                value: "$2a$12$/OzOwInLQj2KIbu9v2cAw.av1KhDs.ey1KbLzpY477aQ8I7muAsaO");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 2,
                column: "ContraseñaHash",
                value: "$2a$12$S8GarW8hKtXpVpL0osCtd.wcXcARszT/gW65RYv8fezAc/OQmlimS");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "hash_admin");

            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "hash_juan");

            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "Id",
                keyValue: 3,
                column: "PasswordHash",
                value: "hash_maria");

            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "Id",
                keyValue: 4,
                column: "PasswordHash",
                value: "hash_carlos");

            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "Id",
                keyValue: 5,
                column: "PasswordHash",
                value: "hash_ana");

            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "Id",
                keyValue: 6,
                column: "PasswordHash",
                value: "hash_pedro");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "ContraseñaHash",
                value: "admin123");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 2,
                column: "ContraseñaHash",
                value: "vendedor123");
        }
    }
}
