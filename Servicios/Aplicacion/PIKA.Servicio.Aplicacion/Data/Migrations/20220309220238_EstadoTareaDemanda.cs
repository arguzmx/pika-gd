using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.AplicacionPlugin.Data.Migrations
{
    public partial class EstadoTareaDemanda : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Estado",
                table: "aplicacion$ondemandtarea",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Estado",
                table: "aplicacion$ondemandtarea");
        }
    }
}
