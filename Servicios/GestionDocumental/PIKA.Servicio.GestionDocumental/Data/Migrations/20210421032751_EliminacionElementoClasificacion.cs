using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.GestionDocumental.Data.Migrations
{
    public partial class EliminacionElementoClasificacion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_gd$entradaclasificacion_gd$tipodisposiciondocumental_TipoDis~",
                table: "gd$entradaclasificacion");

            migrationBuilder.DropForeignKey(
                name: "FK_gd$valoracionentradaclasificacion_gd$entradaclasificacion_En~",
                table: "gd$valoracionentradaclasificacion");

            migrationBuilder.AddForeignKey(
                name: "FK_gd$entradaclasificacion_gd$tipodisposiciondocumental_TipoDis~",
                table: "gd$entradaclasificacion",
                column: "TipoDisposicionDocumentalId",
                principalTable: "gd$tipodisposiciondocumental",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_gd$valoracionentradaclasificacion_gd$entradaclasificacion_En~",
                table: "gd$valoracionentradaclasificacion",
                column: "EntradaClasificacionId",
                principalTable: "gd$entradaclasificacion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_gd$entradaclasificacion_gd$tipodisposiciondocumental_TipoDis~",
                table: "gd$entradaclasificacion");

            migrationBuilder.DropForeignKey(
                name: "FK_gd$valoracionentradaclasificacion_gd$entradaclasificacion_En~",
                table: "gd$valoracionentradaclasificacion");

            migrationBuilder.AddForeignKey(
                name: "FK_gd$entradaclasificacion_gd$tipodisposiciondocumental_TipoDis~",
                table: "gd$entradaclasificacion",
                column: "TipoDisposicionDocumentalId",
                principalTable: "gd$tipodisposiciondocumental",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_gd$valoracionentradaclasificacion_gd$entradaclasificacion_En~",
                table: "gd$valoracionentradaclasificacion",
                column: "EntradaClasificacionId",
                principalTable: "gd$entradaclasificacion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
