using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Contenido.data.Migrations
{
    public partial class Cambiotypolongitudbyte : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "LongitudBytes",
                table: "contenido$versionpartes",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "LongitudBytes",
                table: "contenido$versionpartes",
                type: "int",
                nullable: false,
                oldClrType: typeof(long));
        }
    }
}
