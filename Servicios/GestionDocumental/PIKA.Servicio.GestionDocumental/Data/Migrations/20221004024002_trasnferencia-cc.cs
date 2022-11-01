using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.GestionDocumental.data.migrations
{
    public partial class trasnferenciacc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Folio",
                table: "gd$transferencia",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CuadroClasificacionId",
                table: "gd$transferencia",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EntradaClasificacionId",
                table: "gd$transferencia",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_gd$transferencia_CuadroClasificacionId",
                table: "gd$transferencia",
                column: "CuadroClasificacionId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$transferencia_EntradaClasificacionId",
                table: "gd$transferencia",
                column: "EntradaClasificacionId");

            migrationBuilder.AddForeignKey(
                name: "FK_gd$transferencia_gd$cuadroclasificacion_CuadroClasificacionId",
                table: "gd$transferencia",
                column: "CuadroClasificacionId",
                principalTable: "gd$cuadroclasificacion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_gd$transferencia_gd$entradaclasificacion_EntradaClasificacio~",
                table: "gd$transferencia",
                column: "EntradaClasificacionId",
                principalTable: "gd$entradaclasificacion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_gd$transferencia_gd$cuadroclasificacion_CuadroClasificacionId",
                table: "gd$transferencia");

            migrationBuilder.DropForeignKey(
                name: "FK_gd$transferencia_gd$entradaclasificacion_EntradaClasificacio~",
                table: "gd$transferencia");

            migrationBuilder.DropIndex(
                name: "IX_gd$transferencia_CuadroClasificacionId",
                table: "gd$transferencia");

            migrationBuilder.DropIndex(
                name: "IX_gd$transferencia_EntradaClasificacionId",
                table: "gd$transferencia");

            migrationBuilder.DropColumn(
                name: "CuadroClasificacionId",
                table: "gd$transferencia");

            migrationBuilder.DropColumn(
                name: "EntradaClasificacionId",
                table: "gd$transferencia");

            migrationBuilder.AlterColumn<string>(
                name: "Folio",
                table: "gd$transferencia",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 500,
                oldNullable: true);
        }
    }
}
