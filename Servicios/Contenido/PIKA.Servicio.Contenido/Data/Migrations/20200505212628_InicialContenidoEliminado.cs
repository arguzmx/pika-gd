using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Contenido.Data.Migrations
{
    public partial class InicialContenidoEliminado : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaCreacion",
                table: "Contenido$versionelemento",
                nullable: false,
                defaultValue: new DateTime(2020, 5, 5, 16, 26, 27, 773, DateTimeKind.Local).AddTicks(5848),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2020, 4, 30, 18, 14, 42, 925, DateTimeKind.Local).AddTicks(8711));

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaActualizacion",
                table: "Contenido$versionelemento",
                nullable: false,
                defaultValue: new DateTime(2020, 5, 5, 16, 26, 27, 773, DateTimeKind.Local).AddTicks(6274),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2020, 4, 30, 18, 14, 42, 925, DateTimeKind.Local).AddTicks(9167));

            migrationBuilder.AddColumn<bool>(
                name: "Eliminada",
                table: "Contenido$versionelemento",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Eliminada",
                table: "Contenido$tipogestores",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaCreacion",
                table: "Contenido$elemento",
                nullable: false,
                defaultValue: new DateTime(2020, 5, 5, 16, 26, 27, 746, DateTimeKind.Local).AddTicks(3544),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2020, 4, 30, 18, 14, 42, 899, DateTimeKind.Local).AddTicks(7086));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Eliminada",
                table: "Contenido$versionelemento");

            migrationBuilder.DropColumn(
                name: "Eliminada",
                table: "Contenido$tipogestores");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaCreacion",
                table: "Contenido$versionelemento",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2020, 4, 30, 18, 14, 42, 925, DateTimeKind.Local).AddTicks(8711),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 5, 5, 16, 26, 27, 773, DateTimeKind.Local).AddTicks(5848));

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaActualizacion",
                table: "Contenido$versionelemento",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2020, 4, 30, 18, 14, 42, 925, DateTimeKind.Local).AddTicks(9167),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 5, 5, 16, 26, 27, 773, DateTimeKind.Local).AddTicks(6274));

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaCreacion",
                table: "Contenido$elemento",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2020, 4, 30, 18, 14, 42, 899, DateTimeKind.Local).AddTicks(7086),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 5, 5, 16, 26, 27, 746, DateTimeKind.Local).AddTicks(3544));
        }
    }
}
