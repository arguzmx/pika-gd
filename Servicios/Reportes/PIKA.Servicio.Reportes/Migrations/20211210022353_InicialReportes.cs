using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Reportes.Migrations
{
    public partial class InicialReportes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExtensionSalida",
                table: "repo$reporteentidad",
                maxLength: 64,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExtensionSalida",
                table: "repo$reporteentidad");
        }
    }
}
