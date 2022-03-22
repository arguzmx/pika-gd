using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Contenido.Data.Migrations
{
    public partial class TamanoBytesElemento : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "TamanoBytes",
                table: "contenido$elemento",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_contenido$elemento_ConteoAnexos",
                table: "contenido$elemento",
                column: "ConteoAnexos");

            migrationBuilder.CreateIndex(
                name: "IX_contenido$elemento_TamanoBytes",
                table: "contenido$elemento",
                column: "TamanoBytes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_contenido$elemento_ConteoAnexos",
                table: "contenido$elemento");

            migrationBuilder.DropIndex(
                name: "IX_contenido$elemento_TamanoBytes",
                table: "contenido$elemento");

            migrationBuilder.DropColumn(
                name: "TamanoBytes",
                table: "contenido$elemento");
        }
    }
}
