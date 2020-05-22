using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Metadatos.Data.Migrations
{
    public partial class RefinamientoMetadatos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_metadatos$asociacionplantilla_metadatos$plantilla_Id",
                table: "metadatos$asociacionplantilla");

            migrationBuilder.DropForeignKey(
                name: "FK_metadatos$propiedadplantilla_metadatos$atributotabla_Atribut~",
                table: "metadatos$propiedadplantilla");

            migrationBuilder.DropTable(
                name: "metadatos$atributometadato");

            migrationBuilder.DropTable(
                name: "metadatos$tipodatopropiedadplantilla");

            migrationBuilder.DropIndex(
                name: "IX_metadatos$propiedadplantilla_AtributoTablaid",
                table: "metadatos$propiedadplantilla");

            migrationBuilder.DropColumn(
                name: "AtributoTablaid",
                table: "metadatos$propiedadplantilla");

            migrationBuilder.DropColumn(
                name: "TipoDatoProiedadPlantillaId",
                table: "metadatos$propiedadplantilla");

            migrationBuilder.AddColumn<string>(
                name: "ValorDefault",
                table: "metadatos$propiedadplantilla",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_metadatos$propiedadplantilla_TipoDatoId",
                table: "metadatos$propiedadplantilla",
                column: "TipoDatoId");

            migrationBuilder.CreateIndex(
                name: "IX_metadatos$atributotabla_PropiedadId",
                table: "metadatos$atributotabla",
                column: "PropiedadId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_metadatos$asociacionplantilla_PlantillaId",
                table: "metadatos$asociacionplantilla",
                column: "PlantillaId");

            migrationBuilder.AddForeignKey(
                name: "FK_metadatos$asociacionplantilla_metadatos$plantilla_PlantillaId",
                table: "metadatos$asociacionplantilla",
                column: "PlantillaId",
                principalTable: "metadatos$plantilla",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_metadatos$atributotabla_metadatos$propiedadplantilla_Propied~",
                table: "metadatos$atributotabla",
                column: "PropiedadId",
                principalTable: "metadatos$propiedadplantilla",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_metadatos$propiedadplantilla_metadatos$tipodato_TipoDatoId",
                table: "metadatos$propiedadplantilla",
                column: "TipoDatoId",
                principalTable: "metadatos$tipodato",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_metadatos$asociacionplantilla_metadatos$plantilla_PlantillaId",
                table: "metadatos$asociacionplantilla");

            migrationBuilder.DropForeignKey(
                name: "FK_metadatos$atributotabla_metadatos$propiedadplantilla_Propied~",
                table: "metadatos$atributotabla");

            migrationBuilder.DropForeignKey(
                name: "FK_metadatos$propiedadplantilla_metadatos$tipodato_TipoDatoId",
                table: "metadatos$propiedadplantilla");

            migrationBuilder.DropIndex(
                name: "IX_metadatos$propiedadplantilla_TipoDatoId",
                table: "metadatos$propiedadplantilla");

            migrationBuilder.DropIndex(
                name: "IX_metadatos$atributotabla_PropiedadId",
                table: "metadatos$atributotabla");

            migrationBuilder.DropIndex(
                name: "IX_metadatos$asociacionplantilla_PlantillaId",
                table: "metadatos$asociacionplantilla");

            migrationBuilder.DropColumn(
                name: "ValorDefault",
                table: "metadatos$propiedadplantilla");

            migrationBuilder.AddColumn<string>(
                name: "AtributoTablaid",
                table: "metadatos$propiedadplantilla",
                type: "varchar(255) CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipoDatoProiedadPlantillaId",
                table: "metadatos$propiedadplantilla",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "metadatos$atributometadato",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false),
                    PropiedadId = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false),
                    PropiedadPlantillaId1 = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_metadatos$atributometadato", x => x.Id);
                    table.ForeignKey(
                        name: "FK_metadatos$atributometadato_metadatos$propiedadplantilla_Prop~",
                        column: x => x.PropiedadId,
                        principalTable: "metadatos$propiedadplantilla",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_metadatos$atributometadato_metadatos$propiedadplantilla_Pro~1",
                        column: x => x.PropiedadPlantillaId1,
                        principalTable: "metadatos$propiedadplantilla",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "metadatos$tipodatopropiedadplantilla",
                columns: table => new
                {
                    TipoDatoId = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false),
                    PropiedadPlantillaId = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_metadatos$tipodatopropiedadplantilla", x => new { x.TipoDatoId, x.PropiedadPlantillaId });
                    table.ForeignKey(
                        name: "FK_metadatos$tipodatopropiedadplantilla_metadatos$propiedadplan~",
                        column: x => x.PropiedadPlantillaId,
                        principalTable: "metadatos$propiedadplantilla",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_metadatos$tipodatopropiedadplantilla_metadatos$tipodato_Tipo~",
                        column: x => x.TipoDatoId,
                        principalTable: "metadatos$tipodato",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_metadatos$propiedadplantilla_AtributoTablaid",
                table: "metadatos$propiedadplantilla",
                column: "AtributoTablaid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_metadatos$atributometadato_PropiedadId",
                table: "metadatos$atributometadato",
                column: "PropiedadId");

            migrationBuilder.CreateIndex(
                name: "IX_metadatos$atributometadato_PropiedadPlantillaId1",
                table: "metadatos$atributometadato",
                column: "PropiedadPlantillaId1");

            migrationBuilder.CreateIndex(
                name: "IX_metadatos$tipodatopropiedadplantilla_PropiedadPlantillaId",
                table: "metadatos$tipodatopropiedadplantilla",
                column: "PropiedadPlantillaId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_metadatos$asociacionplantilla_metadatos$plantilla_Id",
                table: "metadatos$asociacionplantilla",
                column: "Id",
                principalTable: "metadatos$plantilla",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_metadatos$propiedadplantilla_metadatos$atributotabla_Atribut~",
                table: "metadatos$propiedadplantilla",
                column: "AtributoTablaid",
                principalTable: "metadatos$atributotabla",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
