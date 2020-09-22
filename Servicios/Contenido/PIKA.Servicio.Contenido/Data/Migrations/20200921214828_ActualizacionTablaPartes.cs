using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Contenido.data.Migrations
{
    public partial class ActualizacionTablaPartes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_contenido$versionpartes_contenido$elemento_ElementoId",
                table: "contenido$versionpartes");

            migrationBuilder.DropForeignKey(
                name: "FK_contenido$versionpartes_contenido$versionelemento_VersionId",
                table: "contenido$versionpartes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_contenido$versionpartes",
                table: "contenido$versionpartes");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "contenido$versionpartes",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_contenido$versionpartes",
                table: "contenido$versionpartes",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_contenido$versionpartes_ElementoId",
                table: "contenido$versionpartes",
                column: "ElementoId");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_contenido$versionpartes_contenido$elemento_ElementoId",
                table: "contenido$versionpartes");

            migrationBuilder.DropForeignKey(
                name: "FK_contenido$versionpartes_contenido$versionelemento_VersionId",
                table: "contenido$versionpartes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_contenido$versionpartes",
                table: "contenido$versionpartes");

            migrationBuilder.DropIndex(
                name: "IX_contenido$versionpartes_ElementoId",
                table: "contenido$versionpartes");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "contenido$versionpartes",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 128);

            migrationBuilder.AddPrimaryKey(
                name: "PK_contenido$versionpartes",
                table: "contenido$versionpartes",
                columns: new[] { "ElementoId", "VersionId" });

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
    }
}
