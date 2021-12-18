using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Reportes.Migrations
{
    public partial class BloqueoReportes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Bloqueado",
                table: "repo$reporteentidad",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bloqueado",
                table: "repo$reporteentidad");
        }
    }
}
