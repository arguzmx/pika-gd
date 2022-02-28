using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.GestionDocumental.Data.Migrations
{
    public partial class UbicacionBAsica : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UbicacionCaja",
                table: "gd$activo",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UbicacionRack",
                table: "gd$activo",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_gd$activo_UbicacionCaja",
                table: "gd$activo",
                column: "UbicacionCaja");

            migrationBuilder.CreateIndex(
                name: "IX_gd$activo_UbicacionRack",
                table: "gd$activo",
                column: "UbicacionRack");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_gd$activo_UbicacionCaja",
                table: "gd$activo");

            migrationBuilder.DropIndex(
                name: "IX_gd$activo_UbicacionRack",
                table: "gd$activo");

            migrationBuilder.DropColumn(
                name: "UbicacionCaja",
                table: "gd$activo");

            migrationBuilder.DropColumn(
                name: "UbicacionRack",
                table: "gd$activo");
        }
    }
}
