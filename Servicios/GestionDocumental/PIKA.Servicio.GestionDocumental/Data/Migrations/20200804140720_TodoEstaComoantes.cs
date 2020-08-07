using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.GestionDocumental.Data.Migrations
{
    public partial class TodoEstaComoantes : Migration
    {
       
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_gd$activo_gd$archivo_ArchivoId",
                table: "gd$activo");

            migrationBuilder.DropForeignKey(
                name: "FK_gd$ampliacion_gd$tipoampliacion_TipoAmpliacionId",
                table: "gd$ampliacion");

            migrationBuilder.DropForeignKey(
                name: "FK_gd$archivo_gd$tipoarchivo_TipoArchivoId",
                table: "gd$archivo");

            migrationBuilder.AddForeignKey(
                name: "FK_gd$activo_gd$archivo_ArchivoId",
                table: "gd$activo",
                column: "ArchivoId",
                principalTable: "gd$archivo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_gd$ampliacion_gd$tipoampliacion_TipoAmpliacionId",
                table: "gd$ampliacion",
                column: "TipoAmpliacionId",
                principalTable: "gd$tipoampliacion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_gd$archivo_gd$tipoarchivo_TipoArchivoId",
                table: "gd$archivo",
                column: "TipoArchivoId",
                principalTable: "gd$tipoarchivo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_gd$activo_gd$archivo_ArchivoId",
                table: "gd$activo");

            migrationBuilder.DropForeignKey(
                name: "FK_gd$ampliacion_gd$tipoampliacion_TipoAmpliacionId",
                table: "gd$ampliacion");

            migrationBuilder.DropForeignKey(
                name: "FK_gd$archivo_gd$tipoarchivo_TipoArchivoId",
                table: "gd$archivo");

            migrationBuilder.AddForeignKey(
                name: "FK_gd$activo_gd$archivo_ArchivoId",
                table: "gd$activo",
                column: "ArchivoId",
                principalTable: "gd$archivo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_gd$ampliacion_gd$tipoampliacion_TipoAmpliacionId",
                table: "gd$ampliacion",
                column: "TipoAmpliacionId",
                principalTable: "gd$tipoampliacion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_gd$archivo_gd$tipoarchivo_TipoArchivoId",
                table: "gd$archivo",
                column: "TipoArchivoId",
                principalTable: "gd$tipoarchivo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
