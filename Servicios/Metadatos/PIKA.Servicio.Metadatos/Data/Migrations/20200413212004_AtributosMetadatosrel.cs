using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Metadatos.Data.Migrations
{
    public partial class AtributosMetadatosrel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_metadatos$atributometadato_metadatos$propiedadplantilla_Id",
                table: "metadatos$atributometadato");

            migrationBuilder.DropForeignKey(
                name: "FK_metadatos$atributometadato_metadatos$propiedadplantilla_Prop~",
                table: "metadatos$atributometadato");

            migrationBuilder.CreateIndex(
                name: "IX_metadatos$atributometadato_PropiedadId",
                table: "metadatos$atributometadato",
                column: "PropiedadId");

            migrationBuilder.AddForeignKey(
                name: "FK_metadatos$atributometadato_metadatos$propiedadplantilla_Prop~",
                table: "metadatos$atributometadato",
                column: "PropiedadId",
                principalTable: "metadatos$propiedadplantilla",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_metadatos$atributometadato_metadatos$propiedadplantilla_Pro~1",
                table: "metadatos$atributometadato",
                column: "PropiedadPlantillaId1",
                principalTable: "metadatos$propiedadplantilla",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_metadatos$atributometadato_metadatos$propiedadplantilla_Prop~",
                table: "metadatos$atributometadato");

            migrationBuilder.DropForeignKey(
                name: "FK_metadatos$atributometadato_metadatos$propiedadplantilla_Pro~1",
                table: "metadatos$atributometadato");

            migrationBuilder.DropIndex(
                name: "IX_metadatos$atributometadato_PropiedadId",
                table: "metadatos$atributometadato");

            migrationBuilder.AddForeignKey(
                name: "FK_metadatos$atributometadato_metadatos$propiedadplantilla_Id",
                table: "metadatos$atributometadato",
                column: "Id",
                principalTable: "metadatos$propiedadplantilla",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_metadatos$atributometadato_metadatos$propiedadplantilla_Prop~",
                table: "metadatos$atributometadato",
                column: "PropiedadPlantillaId1",
                principalTable: "metadatos$propiedadplantilla",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
