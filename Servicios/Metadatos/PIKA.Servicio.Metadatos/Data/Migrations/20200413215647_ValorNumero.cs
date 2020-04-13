using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Metadatos.Data.Migrations
{
    public partial class ValorNumero : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_metadatos$validadornumero_metadatos$propiedadplantilla_Id",
                table: "metadatos$validadornumero");

            migrationBuilder.DropForeignKey(
                name: "FK_metadatos$validadortexto_metadatos$propiedadplantilla_Id",
                table: "metadatos$validadortexto");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "metadatos$validadortexto",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(128) CHARACTER SET utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "metadatos$validadornumero",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(128) CHARACTER SET utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_metadatos$validadortexto_PropiedadId",
                table: "metadatos$validadortexto",
                column: "PropiedadId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_metadatos$validadornumero_PropiedadId",
                table: "metadatos$validadornumero",
                column: "PropiedadId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_metadatos$validadornumero_metadatos$propiedadplantilla_Propi~",
                table: "metadatos$validadornumero",
                column: "PropiedadId",
                principalTable: "metadatos$propiedadplantilla",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_metadatos$validadortexto_metadatos$propiedadplantilla_Propie~",
                table: "metadatos$validadortexto",
                column: "PropiedadId",
                principalTable: "metadatos$propiedadplantilla",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_metadatos$validadornumero_metadatos$propiedadplantilla_Propi~",
                table: "metadatos$validadornumero");

            migrationBuilder.DropForeignKey(
                name: "FK_metadatos$validadortexto_metadatos$propiedadplantilla_Propie~",
                table: "metadatos$validadortexto");

            migrationBuilder.DropIndex(
                name: "IX_metadatos$validadortexto_PropiedadId",
                table: "metadatos$validadortexto");

            migrationBuilder.DropIndex(
                name: "IX_metadatos$validadornumero_PropiedadId",
                table: "metadatos$validadornumero");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "metadatos$validadortexto",
                type: "varchar(128) CHARACTER SET utf8mb4",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "metadatos$validadornumero",
                type: "varchar(128) CHARACTER SET utf8mb4",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AddForeignKey(
                name: "FK_metadatos$validadornumero_metadatos$propiedadplantilla_Id",
                table: "metadatos$validadornumero",
                column: "Id",
                principalTable: "metadatos$propiedadplantilla",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_metadatos$validadortexto_metadatos$propiedadplantilla_Id",
                table: "metadatos$validadortexto",
                column: "Id",
                principalTable: "metadatos$propiedadplantilla",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
