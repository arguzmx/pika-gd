using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Contenido.Data.Migrations
{
    public partial class InicialContenidoElemento : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contenido$elemento_Contenido$volumen_VolumenId",
                table: "Contenido$elemento");

            migrationBuilder.DropIndex(
                name: "IX_Contenido$elemento_VolumenId",
                table: "Contenido$elemento");

            migrationBuilder.DropColumn(
                name: "VolumenId",
                table: "Contenido$elemento");

            migrationBuilder.AddColumn<string>(
                name: "Elementoid",
                table: "Contenido$volumen",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaCreacion",
                table: "Contenido$versionelemento",
                nullable: false,
                defaultValue: new DateTime(2020, 4, 30, 18, 14, 42, 925, DateTimeKind.Local).AddTicks(8711),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2020, 4, 30, 15, 47, 47, 950, DateTimeKind.Local).AddTicks(7517));

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaActualizacion",
                table: "Contenido$versionelemento",
                nullable: false,
                defaultValue: new DateTime(2020, 4, 30, 18, 14, 42, 925, DateTimeKind.Local).AddTicks(9167),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2020, 4, 30, 15, 47, 47, 950, DateTimeKind.Local).AddTicks(8286));

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaCreacion",
                table: "Contenido$elemento",
                nullable: false,
                defaultValue: new DateTime(2020, 4, 30, 18, 14, 42, 899, DateTimeKind.Local).AddTicks(7086),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2020, 4, 30, 15, 47, 47, 904, DateTimeKind.Local).AddTicks(954));

            migrationBuilder.CreateIndex(
                name: "IX_Contenido$volumen_Elementoid",
                table: "Contenido$volumen",
                column: "Elementoid",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Contenido$volumen_Contenido$elemento_Elementoid",
                table: "Contenido$volumen",
                column: "Elementoid",
                principalTable: "Contenido$elemento",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contenido$volumen_Contenido$elemento_Elementoid",
                table: "Contenido$volumen");

            migrationBuilder.DropIndex(
                name: "IX_Contenido$volumen_Elementoid",
                table: "Contenido$volumen");

            migrationBuilder.DropColumn(
                name: "Elementoid",
                table: "Contenido$volumen");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaCreacion",
                table: "Contenido$versionelemento",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2020, 4, 30, 15, 47, 47, 950, DateTimeKind.Local).AddTicks(7517),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 4, 30, 18, 14, 42, 925, DateTimeKind.Local).AddTicks(8711));

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaActualizacion",
                table: "Contenido$versionelemento",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2020, 4, 30, 15, 47, 47, 950, DateTimeKind.Local).AddTicks(8286),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 4, 30, 18, 14, 42, 925, DateTimeKind.Local).AddTicks(9167));

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaCreacion",
                table: "Contenido$elemento",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2020, 4, 30, 15, 47, 47, 904, DateTimeKind.Local).AddTicks(954),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 4, 30, 18, 14, 42, 899, DateTimeKind.Local).AddTicks(7086));

            migrationBuilder.AddColumn<string>(
                name: "VolumenId",
                table: "Contenido$elemento",
                type: "varchar(128) CHARACTER SET utf8mb4",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Contenido$elemento_VolumenId",
                table: "Contenido$elemento",
                column: "VolumenId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Contenido$elemento_Contenido$volumen_VolumenId",
                table: "Contenido$elemento",
                column: "VolumenId",
                principalTable: "Contenido$volumen",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
