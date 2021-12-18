using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.GestionDocumental.Data.Migrations
{
    public partial class UnidadesAdministrativasArchivos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ArchivoConcentracionId",
                table: "gd$unidadadministrativaarchivo",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ArchivoHistoricoId",
                table: "gd$unidadadministrativaarchivo",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ArchivoTramiteId",
                table: "gd$unidadadministrativaarchivo",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_gd$unidadadministrativaarchivo_ArchivoConcentracionId",
                table: "gd$unidadadministrativaarchivo",
                column: "ArchivoConcentracionId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$unidadadministrativaarchivo_ArchivoHistoricoId",
                table: "gd$unidadadministrativaarchivo",
                column: "ArchivoHistoricoId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$unidadadministrativaarchivo_ArchivoTramiteId",
                table: "gd$unidadadministrativaarchivo",
                column: "ArchivoTramiteId");

            migrationBuilder.AddForeignKey(
                name: "FK_gd$unidadadministrativaarchivo_gd$archivo_ArchivoConcentraci~",
                table: "gd$unidadadministrativaarchivo",
                column: "ArchivoConcentracionId",
                principalTable: "gd$archivo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_gd$unidadadministrativaarchivo_gd$archivo_ArchivoHistoricoId",
                table: "gd$unidadadministrativaarchivo",
                column: "ArchivoHistoricoId",
                principalTable: "gd$archivo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_gd$unidadadministrativaarchivo_gd$archivo_ArchivoTramiteId",
                table: "gd$unidadadministrativaarchivo",
                column: "ArchivoTramiteId",
                principalTable: "gd$archivo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_gd$unidadadministrativaarchivo_gd$archivo_ArchivoConcentraci~",
                table: "gd$unidadadministrativaarchivo");

            migrationBuilder.DropForeignKey(
                name: "FK_gd$unidadadministrativaarchivo_gd$archivo_ArchivoHistoricoId",
                table: "gd$unidadadministrativaarchivo");

            migrationBuilder.DropForeignKey(
                name: "FK_gd$unidadadministrativaarchivo_gd$archivo_ArchivoTramiteId",
                table: "gd$unidadadministrativaarchivo");

            migrationBuilder.DropIndex(
                name: "IX_gd$unidadadministrativaarchivo_ArchivoConcentracionId",
                table: "gd$unidadadministrativaarchivo");

            migrationBuilder.DropIndex(
                name: "IX_gd$unidadadministrativaarchivo_ArchivoHistoricoId",
                table: "gd$unidadadministrativaarchivo");

            migrationBuilder.DropIndex(
                name: "IX_gd$unidadadministrativaarchivo_ArchivoTramiteId",
                table: "gd$unidadadministrativaarchivo");

            migrationBuilder.DropColumn(
                name: "ArchivoConcentracionId",
                table: "gd$unidadadministrativaarchivo");

            migrationBuilder.DropColumn(
                name: "ArchivoHistoricoId",
                table: "gd$unidadadministrativaarchivo");

            migrationBuilder.DropColumn(
                name: "ArchivoTramiteId",
                table: "gd$unidadadministrativaarchivo");
        }
    }
}
