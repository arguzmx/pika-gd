using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Seguridad.Data.Migrations
{
    public partial class InicialSeguridadDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ReleaseIndex",
                table: "seguridad$aplicacion",
                nullable: false,
                defaultValue: 10,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ReleaseIndex",
                table: "seguridad$aplicacion",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldDefaultValue: 10);
        }
    }
}
