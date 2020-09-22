using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Contenido.data.Migrations
{
    public partial class ActualizacionTablaPartesFKs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_contenido$versionpartes_contenido$elemento_ElementoId",
                table: "contenido$versionpartes");

            migrationBuilder.DropForeignKey(
                name: "FK_contenido$versionpartes_contenido$versionelemento_VersionId",
                table: "contenido$versionpartes");

            migrationBuilder.AddForeignKey(
                name: "FK_contenido$versionpartes_contenido$elemento_ElementoId",
                table: "contenido$versionpartes",
                column: "ElementoId",
                principalTable: "contenido$elemento",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_contenido$versionpartes_contenido$versionelemento_VersionId",
                table: "contenido$versionpartes",
                column: "VersionId",
                principalTable: "contenido$versionelemento",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_contenido$versionpartes_contenido$elemento_ElementoId",
                table: "contenido$versionpartes");

            migrationBuilder.DropForeignKey(
                name: "FK_contenido$versionpartes_contenido$versionelemento_VersionId",
                table: "contenido$versionpartes");

            migrationBuilder.AddForeignKey(
                name: "FK_contenido$versionpartes_contenido$elemento_ElementoId",
                table: "contenido$versionpartes",
                column: "ElementoId",
                principalTable: "contenido$elemento",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_contenido$versionpartes_contenido$versionelemento_VersionId",
                table: "contenido$versionpartes",
                column: "VersionId",
                principalTable: "contenido$versionelemento",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
