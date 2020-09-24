using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Contenido.data.Migrations
{
    public partial class ModificacionParteContenido : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EsImagen",
                table: "contenido$versionpartes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Indexada",
                table: "contenido$versionpartes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "TieneMiniatura",
                table: "contenido$versionpartes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_contenido$versionpartes_Indexada",
                table: "contenido$versionpartes",
                column: "Indexada");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_contenido$versionpartes_Indexada",
                table: "contenido$versionpartes");

            migrationBuilder.DropColumn(
                name: "EsImagen",
                table: "contenido$versionpartes");

            migrationBuilder.DropColumn(
                name: "Indexada",
                table: "contenido$versionpartes");

            migrationBuilder.DropColumn(
                name: "TieneMiniatura",
                table: "contenido$versionpartes");
        }
    }
}
