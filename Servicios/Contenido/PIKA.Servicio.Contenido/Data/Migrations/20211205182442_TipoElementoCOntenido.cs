using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Contenido.Data.Migrations
{
    public partial class TipoElementoCOntenido : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdExterno",
                table: "contenido$elemento",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipoElemento",
                table: "contenido$elemento",
                maxLength: 64,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_contenido$elemento_IdExterno",
                table: "contenido$elemento",
                column: "IdExterno");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_contenido$elemento_IdExterno",
                table: "contenido$elemento");

            migrationBuilder.DropColumn(
                name: "IdExterno",
                table: "contenido$elemento");

            migrationBuilder.DropColumn(
                name: "TipoElemento",
                table: "contenido$elemento");
        }
    }
}
