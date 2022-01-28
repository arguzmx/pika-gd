using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.AplicacionPlugin.Data.Migrations
{
    public partial class ActualizacionTareaAT : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Estado",
                table: "aplicacion$autotarea",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "TareaEjecucionContinua",
                table: "aplicacion$autotarea",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "TareaEjecucionContinuaMinutos",
                table: "aplicacion$autotarea",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Estado",
                table: "aplicacion$autotarea");

            migrationBuilder.DropColumn(
                name: "TareaEjecucionContinua",
                table: "aplicacion$autotarea");

            migrationBuilder.DropColumn(
                name: "TareaEjecucionContinuaMinutos",
                table: "aplicacion$autotarea");
        }
    }
}
