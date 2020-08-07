using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.GestionDocumental.Data.Migrations
{
    public partial class CascadeoArchivo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_gd$activo_gd$archivo_ArchivoId",
                table: "gd$activo");

            migrationBuilder.DropForeignKey(
                name: "FK_gd$historialarchivoactivo_gd$archivo_ArchivoId",
                table: "gd$historialarchivoactivo");

            migrationBuilder.AddForeignKey(
                name: "FK_gd$activo_gd$archivo_ArchivoId",
                table: "gd$activo",
                column: "ArchivoId",
                principalTable: "gd$archivo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_gd$historialarchivoactivo_gd$archivo_ArchivoId",
                table: "gd$historialarchivoactivo",
                column: "ArchivoId",
                principalTable: "gd$archivo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_gd$activo_gd$archivo_ArchivoId",
                table: "gd$activo");

            migrationBuilder.DropForeignKey(
                name: "FK_gd$historialarchivoactivo_gd$archivo_ArchivoId",
                table: "gd$historialarchivoactivo");

            migrationBuilder.AddForeignKey(
                name: "FK_gd$activo_gd$archivo_ArchivoId",
                table: "gd$activo",
                column: "ArchivoId",
                principalTable: "gd$archivo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_gd$historialarchivoactivo_gd$archivo_ArchivoId",
                table: "gd$historialarchivoactivo",
                column: "ArchivoId",
                principalTable: "gd$archivo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
