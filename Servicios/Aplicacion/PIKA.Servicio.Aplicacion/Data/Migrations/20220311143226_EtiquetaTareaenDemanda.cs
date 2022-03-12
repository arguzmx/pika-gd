using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.AplicacionPlugin.Data.Migrations
{
    public partial class EtiquetaTareaenDemanda : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Etiqueta",
                table: "aplicacion$ondemandtarea",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Etiqueta",
                table: "aplicacion$ondemandtarea");
        }
    }
}
