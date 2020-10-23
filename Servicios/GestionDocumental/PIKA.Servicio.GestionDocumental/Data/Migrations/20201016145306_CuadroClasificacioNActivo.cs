using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.GestionDocumental.Data.Migrations
{
    public partial class CuadroClasificacioNActivo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CuadroClasificacionId",
                table: "gd$activo",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_gd$activo_CuadroClasificacionId",
                table: "gd$activo",
                column: "CuadroClasificacionId");

            migrationBuilder.AddForeignKey(
                name: "FK_gd$activo_gd$cuadroclasificacion_CuadroClasificacionId",
                table: "gd$activo",
                column: "CuadroClasificacionId",
                principalTable: "gd$cuadroclasificacion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_gd$activo_gd$cuadroclasificacion_CuadroClasificacionId",
                table: "gd$activo");

            migrationBuilder.DropIndex(
                name: "IX_gd$activo_CuadroClasificacionId",
                table: "gd$activo");

            migrationBuilder.DropColumn(
                name: "CuadroClasificacionId",
                table: "gd$activo");
        }
    }
}
