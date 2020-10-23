using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.GestionDocumental.Data.Migrations
{
    public partial class INdicesActivo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_gd$activo_Eliminada",
                table: "gd$activo",
                column: "Eliminada");

            migrationBuilder.CreateIndex(
                name: "IX_gd$activo_FechaApertura",
                table: "gd$activo",
                column: "FechaApertura");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_gd$activo_Eliminada",
                table: "gd$activo");

            migrationBuilder.DropIndex(
                name: "IX_gd$activo_FechaApertura",
                table: "gd$activo");
        }
    }
}
