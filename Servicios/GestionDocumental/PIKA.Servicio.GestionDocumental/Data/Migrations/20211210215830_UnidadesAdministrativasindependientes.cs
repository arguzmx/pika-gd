using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.GestionDocumental.Data.Migrations
{
    public partial class UnidadesAdministrativasindependientes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_gd$unidadadministrativaarchivo_gd$archivo_ArchivoId",
                table: "gd$unidadadministrativaarchivo");

            migrationBuilder.DropIndex(
                name: "IX_gd$unidadadministrativaarchivo_ArchivoId",
                table: "gd$unidadadministrativaarchivo");

            migrationBuilder.DropColumn(
                name: "ArchivoId",
                table: "gd$unidadadministrativaarchivo");

            migrationBuilder.AddColumn<string>(
                name: "OrigenId",
                table: "gd$unidadadministrativaarchivo",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TipoOrigenId",
                table: "gd$unidadadministrativaarchivo",
                maxLength: 128,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrigenId",
                table: "gd$unidadadministrativaarchivo");

            migrationBuilder.DropColumn(
                name: "TipoOrigenId",
                table: "gd$unidadadministrativaarchivo");

            migrationBuilder.AddColumn<string>(
                name: "ArchivoId",
                table: "gd$unidadadministrativaarchivo",
                type: "varchar(128) CHARACTER SET utf8mb4",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_gd$unidadadministrativaarchivo_ArchivoId",
                table: "gd$unidadadministrativaarchivo",
                column: "ArchivoId");

            migrationBuilder.AddForeignKey(
                name: "FK_gd$unidadadministrativaarchivo_gd$archivo_ArchivoId",
                table: "gd$unidadadministrativaarchivo",
                column: "ArchivoId",
                principalTable: "gd$archivo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
