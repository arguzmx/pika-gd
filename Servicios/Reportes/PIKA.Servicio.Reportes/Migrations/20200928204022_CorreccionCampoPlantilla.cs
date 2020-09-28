using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Reportes.Migrations
{
    public partial class CorreccionCampoPlantilla : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Plantilla",
                table: "repo$reporteentidad",
                maxLength: 429496729,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(512) CHARACTER SET utf8mb4",
                oldMaxLength: 512);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Plantilla",
                table: "repo$reporteentidad",
                type: "varchar(512) CHARACTER SET utf8mb4",
                maxLength: 512,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 429496729);
        }
    }
}
