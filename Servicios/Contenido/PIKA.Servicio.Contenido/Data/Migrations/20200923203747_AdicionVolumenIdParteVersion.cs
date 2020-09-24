using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Contenido.data.Migrations
{
    public partial class AdicionVolumenIdParteVersion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VolumenId",
                table: "contenido$versionpartes",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VolumenId",
                table: "contenido$versionelemento",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_contenido$versionpartes_VolumenId",
                table: "contenido$versionpartes",
                column: "VolumenId");

            migrationBuilder.CreateIndex(
                name: "IX_contenido$versionelemento_VolumenId",
                table: "contenido$versionelemento",
                column: "VolumenId");

            migrationBuilder.AddForeignKey(
                name: "FK_contenido$versionelemento_contenido$volumen_VolumenId",
                table: "contenido$versionelemento",
                column: "VolumenId",
                principalTable: "contenido$volumen",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_contenido$versionpartes_contenido$volumen_VolumenId",
                table: "contenido$versionpartes",
                column: "VolumenId",
                principalTable: "contenido$volumen",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_contenido$versionelemento_contenido$volumen_VolumenId",
                table: "contenido$versionelemento");

            migrationBuilder.DropForeignKey(
                name: "FK_contenido$versionpartes_contenido$volumen_VolumenId",
                table: "contenido$versionpartes");

            migrationBuilder.DropIndex(
                name: "IX_contenido$versionpartes_VolumenId",
                table: "contenido$versionpartes");

            migrationBuilder.DropIndex(
                name: "IX_contenido$versionelemento_VolumenId",
                table: "contenido$versionelemento");

            migrationBuilder.DropColumn(
                name: "VolumenId",
                table: "contenido$versionpartes");

            migrationBuilder.DropColumn(
                name: "VolumenId",
                table: "contenido$versionelemento");
        }
    }
}
