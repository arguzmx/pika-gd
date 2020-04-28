using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.AplicacionPlugin.Data.Migrations
{
    public partial class InicialAplicacionPlugineLIMINADO : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaInstalacion",
                table: "aplicacion$plugininstalado",
                nullable: false,
                defaultValue: new DateTime(2020, 4, 27, 20, 37, 43, 722, DateTimeKind.Local).AddTicks(2141),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2020, 4, 27, 16, 36, 21, 40, DateTimeKind.Local).AddTicks(2788));

            migrationBuilder.AddColumn<bool>(
                name: "Eliminada",
                table: "aplicacion$plugin",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Eliminada",
                table: "aplicacion$plugin");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaInstalacion",
                table: "aplicacion$plugininstalado",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2020, 4, 27, 16, 36, 21, 40, DateTimeKind.Local).AddTicks(2788),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 4, 27, 20, 37, 43, 722, DateTimeKind.Local).AddTicks(2141));
        }
    }
}
