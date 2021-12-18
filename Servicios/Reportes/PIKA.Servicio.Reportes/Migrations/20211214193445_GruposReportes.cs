using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Reportes.Migrations
{
    public partial class GruposReportes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GrupoReportes",
                table: "repo$reporteentidad",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SubReporte",
                table: "repo$reporteentidad",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GrupoReportes",
                table: "repo$reporteentidad");

            migrationBuilder.DropColumn(
                name: "SubReporte",
                table: "repo$reporteentidad");
        }
    }
}
