using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.GestionDocumental.Data.Migrations
{
    public partial class ContedidoActivo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ElementoId",
                table: "gd$activo",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TieneContenido",
                table: "gd$activo",
                nullable: false,
                defaultValue: true);

            migrationBuilder.CreateIndex(
                name: "IX_gd$activo_TieneContenido",
                table: "gd$activo",
                column: "TieneContenido");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_gd$activo_TieneContenido",
                table: "gd$activo");

            migrationBuilder.DropColumn(
                name: "ElementoId",
                table: "gd$activo");

            migrationBuilder.DropColumn(
                name: "TieneContenido",
                table: "gd$activo");
        }
    }
}
