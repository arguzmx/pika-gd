using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Contenido.Data.Migrations
{
    public partial class ConteoContenidActivo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ConteoAnexos",
                table: "contenido$elemento",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConteoAnexos",
                table: "contenido$elemento");
        }
    }
}
