using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.GestionDocumental.Data.Migrations
{
    public partial class UbicacioActivo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AlmacenArchivoId",
                table: "gd$activo",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContenedorAlmacenId",
                table: "gd$activo",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ZonaAlmacenId",
                table: "gd$activo",
                maxLength: 128,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlmacenArchivoId",
                table: "gd$activo");

            migrationBuilder.DropColumn(
                name: "ContenedorAlmacenId",
                table: "gd$activo");

            migrationBuilder.DropColumn(
                name: "ZonaAlmacenId",
                table: "gd$activo");
        }
    }
}
