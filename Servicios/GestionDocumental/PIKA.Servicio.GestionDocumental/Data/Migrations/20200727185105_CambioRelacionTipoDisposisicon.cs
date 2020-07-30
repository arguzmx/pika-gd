using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.GestionDocumental.Data.Migrations
{
    public partial class CambioRelacionTipoDisposisicon : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_gd$entradaclasificacion_gd$tipodisposiciondocumental_TipoDis~",
                table: "gd$entradaclasificacion");

            migrationBuilder.AlterColumn<string>(
                name: "TipoDisposicionDocumentalId",
                table: "gd$entradaclasificacion",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(128) CHARACTER SET utf8mb4",
                oldMaxLength: 128);

            migrationBuilder.AddForeignKey(
                name: "FK_gd$entradaclasificacion_gd$tipodisposiciondocumental_TipoDis~",
                table: "gd$entradaclasificacion",
                column: "TipoDisposicionDocumentalId",
                principalTable: "gd$tipodisposiciondocumental",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_gd$entradaclasificacion_gd$tipodisposiciondocumental_TipoDis~",
                table: "gd$entradaclasificacion");

            migrationBuilder.AlterColumn<string>(
                name: "TipoDisposicionDocumentalId",
                table: "gd$entradaclasificacion",
                type: "varchar(128) CHARACTER SET utf8mb4",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_gd$entradaclasificacion_gd$tipodisposiciondocumental_TipoDis~",
                table: "gd$entradaclasificacion",
                column: "TipoDisposicionDocumentalId",
                principalTable: "gd$tipodisposiciondocumental",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
