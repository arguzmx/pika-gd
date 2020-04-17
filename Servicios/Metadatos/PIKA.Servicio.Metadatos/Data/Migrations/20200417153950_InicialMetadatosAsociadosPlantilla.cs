using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Metadatos.Data.Migrations
{
    public partial class InicialMetadatosAsociadosPlantilla : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_metadatos$asociacionplantilla_metadatos$tipoalmacenmetadatos~",
                table: "metadatos$asociacionplantilla");

            migrationBuilder.CreateIndex(
                name: "IX_metadatos$asociacionplantilla_TipoAlmacenMetadatosId",
                table: "metadatos$asociacionplantilla",
                column: "TipoAlmacenMetadatosId");

            migrationBuilder.AddForeignKey(
                name: "FK_metadatos$asociacionplantilla_metadatos$tipoalmacenmetadatos~",
                table: "metadatos$asociacionplantilla",
                column: "TipoAlmacenMetadatosId",
                principalTable: "metadatos$tipoalmacenmetadatos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_metadatos$asociacionplantilla_metadatos$tipoalmacenmetadatos~",
                table: "metadatos$asociacionplantilla");

            migrationBuilder.DropIndex(
                name: "IX_metadatos$asociacionplantilla_TipoAlmacenMetadatosId",
                table: "metadatos$asociacionplantilla");

            migrationBuilder.AddForeignKey(
                name: "FK_metadatos$asociacionplantilla_metadatos$tipoalmacenmetadatos~",
                table: "metadatos$asociacionplantilla",
                column: "Id",
                principalTable: "metadatos$tipoalmacenmetadatos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
