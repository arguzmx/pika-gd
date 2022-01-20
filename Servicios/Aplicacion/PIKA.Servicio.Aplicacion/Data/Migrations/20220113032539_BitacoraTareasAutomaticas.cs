using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.AplicacionPlugin.Data.Migrations
{
    public partial class BitacoraTareasAutomaticas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_aplicacion$bitacoratarea_aplicacion$autotarea_BitacoraTareaId",
                table: "aplicacion$bitacoratarea");

            migrationBuilder.DropIndex(
                name: "IX_aplicacion$bitacoratarea_BitacoraTareaId",
                table: "aplicacion$bitacoratarea");

            migrationBuilder.DropColumn(
                name: "BitacoraTareaId",
                table: "aplicacion$bitacoratarea");

            migrationBuilder.DropColumn(
                name: "UltimaEjecucion",
                table: "aplicacion$bitacoratarea");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaEjecucion",
                table: "aplicacion$bitacoratarea",
                nullable: false,
                defaultValue: new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "TareaId",
                table: "aplicacion$bitacoratarea",
                maxLength: 128,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_aplicacion$bitacoratarea_TareaId",
                table: "aplicacion$bitacoratarea",
                column: "TareaId");

            migrationBuilder.AddForeignKey(
                name: "FK_aplicacion$bitacoratarea_aplicacion$autotarea_TareaId",
                table: "aplicacion$bitacoratarea",
                column: "TareaId",
                principalTable: "aplicacion$autotarea",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_aplicacion$bitacoratarea_aplicacion$autotarea_TareaId",
                table: "aplicacion$bitacoratarea");

            migrationBuilder.DropIndex(
                name: "IX_aplicacion$bitacoratarea_TareaId",
                table: "aplicacion$bitacoratarea");

            migrationBuilder.DropColumn(
                name: "FechaEjecucion",
                table: "aplicacion$bitacoratarea");

            migrationBuilder.DropColumn(
                name: "TareaId",
                table: "aplicacion$bitacoratarea");

            migrationBuilder.AddColumn<string>(
                name: "BitacoraTareaId",
                table: "aplicacion$bitacoratarea",
                type: "varchar(128) CHARACTER SET utf8mb4",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UltimaEjecucion",
                table: "aplicacion$bitacoratarea",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_aplicacion$bitacoratarea_BitacoraTareaId",
                table: "aplicacion$bitacoratarea",
                column: "BitacoraTareaId");

            migrationBuilder.AddForeignKey(
                name: "FK_aplicacion$bitacoratarea_aplicacion$autotarea_BitacoraTareaId",
                table: "aplicacion$bitacoratarea",
                column: "BitacoraTareaId",
                principalTable: "aplicacion$autotarea",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
