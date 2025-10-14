using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CAASD.Integracion.Migrations
{
    /// <inheritdoc />
    public partial class SeedSuperadminUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "Activo", "Apellido", "Cedula", "DireccionCompleta", "Email", "FechaRegistro", "Nombre", "PasswordHash", "RolId", "Telefono", "UltimoAcceso" },
                values: new object[] { 9999, true, "CAASD", "00000000000", "Sede CAASD", "superadmin@caasd.gob.do", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Root", "c8ca302fe651ad24cbfd711fd4b5e37533660cab1764b454cc4182c76b94224c", 1, "809-000-0000", null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 9999);
        }
    }
}
