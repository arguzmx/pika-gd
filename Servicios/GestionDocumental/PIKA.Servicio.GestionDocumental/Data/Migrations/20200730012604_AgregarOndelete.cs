using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.GestionDocumental.Data.Migrations
{
    public partial class AgregarOndelete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_gd$cuadroclasificacion_gd$estadocuadroclasificacion_EstadoCu~",
                table: "gd$cuadroclasificacion");

            migrationBuilder.DropForeignKey(
                name: "FK_gd$elementoclasificacion_gd$elementoclasificacion_ElementoCl~",
                table: "gd$elementoclasificacion");

            migrationBuilder.DropForeignKey(
                name: "FK_gd$valoracionentradaclasificacion_gd$entradaclasificacion_En~",
                table: "gd$valoracionentradaclasificacion");

            migrationBuilder.DropForeignKey(
                name: "FK_gd$valoracionentradaclasificacion_gd$tipovaloraciondocumenta~",
                table: "gd$valoracionentradaclasificacion");

            migrationBuilder.AddForeignKey(
                name: "FK_gd$cuadroclasificacion_gd$estadocuadroclasificacion_EstadoCu~",
                table: "gd$cuadroclasificacion",
                column: "EstadoCuadroClasificacionId",
                principalTable: "gd$estadocuadroclasificacion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_gd$elementoclasificacion_gd$elementoclasificacion_ElementoCl~",
                table: "gd$elementoclasificacion",
                column: "ElementoClasificacionId",
                principalTable: "gd$elementoclasificacion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_gd$valoracionentradaclasificacion_gd$entradaclasificacion_En~",
                table: "gd$valoracionentradaclasificacion",
                column: "EntradaClasificacionId",
                principalTable: "gd$entradaclasificacion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_gd$valoracionentradaclasificacion_gd$tipovaloraciondocumenta~",
                table: "gd$valoracionentradaclasificacion",
                column: "TipoValoracionDocumentalId",
                principalTable: "gd$tipovaloraciondocumental",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_gd$cuadroclasificacion_gd$estadocuadroclasificacion_EstadoCu~",
                table: "gd$cuadroclasificacion");

            migrationBuilder.DropForeignKey(
                name: "FK_gd$elementoclasificacion_gd$elementoclasificacion_ElementoCl~",
                table: "gd$elementoclasificacion");

            migrationBuilder.DropForeignKey(
                name: "FK_gd$valoracionentradaclasificacion_gd$entradaclasificacion_En~",
                table: "gd$valoracionentradaclasificacion");

            migrationBuilder.DropForeignKey(
                name: "FK_gd$valoracionentradaclasificacion_gd$tipovaloraciondocumenta~",
                table: "gd$valoracionentradaclasificacion");

            migrationBuilder.AddForeignKey(
                name: "FK_gd$cuadroclasificacion_gd$estadocuadroclasificacion_EstadoCu~",
                table: "gd$cuadroclasificacion",
                column: "EstadoCuadroClasificacionId",
                principalTable: "gd$estadocuadroclasificacion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_gd$elementoclasificacion_gd$elementoclasificacion_ElementoCl~",
                table: "gd$elementoclasificacion",
                column: "ElementoClasificacionId",
                principalTable: "gd$elementoclasificacion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_gd$valoracionentradaclasificacion_gd$entradaclasificacion_En~",
                table: "gd$valoracionentradaclasificacion",
                column: "EntradaClasificacionId",
                principalTable: "gd$entradaclasificacion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_gd$valoracionentradaclasificacion_gd$tipovaloraciondocumenta~",
                table: "gd$valoracionentradaclasificacion",
                column: "TipoValoracionDocumentalId",
                principalTable: "gd$tipovaloraciondocumental",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
