using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.GestionDocumental.data.migrations
{
    public partial class CorreccionEntradaActivo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_gd$activo_gd$elementoclasificacion_ElementoClasificacionId",
                table: "gd$activo");

            migrationBuilder.DropIndex(
                name: "IX_gd$activo_ElementoClasificacionId",
                table: "gd$activo");

            migrationBuilder.DropColumn(
                name: "ElementoClasificacionId",
                table: "gd$activo");

            migrationBuilder.AddColumn<string>(
                name: "EntradaClasificacionId",
                table: "gd$activo",
                maxLength: 128,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_gd$activo_EntradaClasificacionId",
                table: "gd$activo",
                column: "EntradaClasificacionId");

            migrationBuilder.AddForeignKey(
                name: "FK_gd$activo_gd$entradaclasificacion_EntradaClasificacionId",
                table: "gd$activo",
                column: "EntradaClasificacionId",
                principalTable: "gd$entradaclasificacion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_gd$activo_gd$entradaclasificacion_EntradaClasificacionId",
                table: "gd$activo");

            migrationBuilder.DropIndex(
                name: "IX_gd$activo_EntradaClasificacionId",
                table: "gd$activo");

            migrationBuilder.DropColumn(
                name: "EntradaClasificacionId",
                table: "gd$activo");

            migrationBuilder.AddColumn<string>(
                name: "ElementoClasificacionId",
                table: "gd$activo",
                type: "varchar(128) CHARACTER SET utf8mb4",
                maxLength: 128,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_gd$activo_ElementoClasificacionId",
                table: "gd$activo",
                column: "ElementoClasificacionId");

            migrationBuilder.AddForeignKey(
                name: "FK_gd$activo_gd$elementoclasificacion_ElementoClasificacionId",
                table: "gd$activo",
                column: "ElementoClasificacionId",
                principalTable: "gd$elementoclasificacion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
