using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Contenido.Data.Migrations
{
    public partial class SoporteContedidoActivo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OrigenId",
                table: "contenido$elemento",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TipoOrigenId",
                table: "contenido$elemento",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_contenido$elemento_TipoOrigenId_OrigenId",
                table: "contenido$elemento",
                columns: new[] { "TipoOrigenId", "OrigenId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_contenido$elemento_TipoOrigenId_OrigenId",
                table: "contenido$elemento");

            migrationBuilder.DropColumn(
                name: "OrigenId",
                table: "contenido$elemento");

            migrationBuilder.DropColumn(
                name: "TipoOrigenId",
                table: "contenido$elemento");
        }
    }
}
