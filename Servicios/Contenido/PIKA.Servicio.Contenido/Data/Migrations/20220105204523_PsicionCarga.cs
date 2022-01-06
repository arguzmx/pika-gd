using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Contenido.Data.Migrations
{
    public partial class PsicionCarga : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Posicion",
                table: "contenido$ElementoTransaccionContenido",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PosicionInicio",
                table: "contenido$ElementoTransaccionContenido",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Posicion",
                table: "contenido$ElementoTransaccionContenido");

            migrationBuilder.DropColumn(
                name: "PosicionInicio",
                table: "contenido$ElementoTransaccionContenido");
        }
    }
}
