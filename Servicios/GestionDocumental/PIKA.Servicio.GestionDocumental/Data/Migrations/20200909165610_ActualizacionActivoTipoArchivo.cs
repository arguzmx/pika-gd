using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.GestionDocumental.data.Migrations
{
    public partial class ActualizacionActivoTipoArchivo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TipoArchivoId",
                table: "gd$activo",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_gd$activo_TipoArchivoId",
                table: "gd$activo",
                column: "TipoArchivoId");

            migrationBuilder.AddForeignKey(
                name: "FK_gd$activo_gd$tipoarchivo_TipoArchivoId",
                table: "gd$activo",
                column: "TipoArchivoId",
                principalTable: "gd$tipoarchivo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_gd$activo_gd$tipoarchivo_TipoArchivoId",
                table: "gd$activo");

            migrationBuilder.DropIndex(
                name: "IX_gd$activo_TipoArchivoId",
                table: "gd$activo");

            migrationBuilder.DropColumn(
                name: "TipoArchivoId",
                table: "gd$activo");
        }
    }
}
