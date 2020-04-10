using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Metadatos.Data.Migrations
{
    public partial class PropiedadesPlantilla : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "metadatos$plantilla",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false),
                    TipoOrigenId = table.Column<string>(maxLength: 128, nullable: false),
                    OrigenId = table.Column<string>(maxLength: 128, nullable: false),
                    Eliminada = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_metadatos$plantilla", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "metadatos$tipodato",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_metadatos$tipodato", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "metadatos$propiedadplantilla",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false),
                    TipoDatoId = table.Column<string>(maxLength: 128, nullable: false),
                    IndiceOrdenamiento = table.Column<int>(nullable: false),
                    Buscable = table.Column<bool>(nullable: false, defaultValue: true),
                    Ordenable = table.Column<bool>(nullable: false, defaultValue: false),
                    Visible = table.Column<bool>(nullable: false, defaultValue: true),
                    EsIdClaveExterna = table.Column<bool>(nullable: false, defaultValue: false),
                    EsIdRegistro = table.Column<bool>(nullable: false, defaultValue: false),
                    EsIdJerarquia = table.Column<bool>(nullable: false, defaultValue: false),
                    EsTextoJerarquia = table.Column<bool>(nullable: false, defaultValue: false),
                    EsIdPadreJerarquia = table.Column<bool>(nullable: false, defaultValue: false),
                    EsFiltroJerarquia = table.Column<bool>(nullable: false, defaultValue: false),
                    Requerido = table.Column<bool>(nullable: false, defaultValue: false),
                    Autogenerado = table.Column<bool>(nullable: false, defaultValue: false),
                    EsIndice = table.Column<bool>(nullable: false, defaultValue: false),
                    ControlHTML = table.Column<string>(maxLength: 128, nullable: false),
                    PlantillaId = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_metadatos$propiedadplantilla", x => x.Id);
                    table.ForeignKey(
                        name: "FK_metadatos$propiedadplantilla_metadatos$plantilla_PlantillaId",
                        column: x => x.PlantillaId,
                        principalTable: "metadatos$plantilla",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "metadatos$tipodatopropiedadplantilla",
                columns: table => new
                {
                    TipoDatoId = table.Column<string>(maxLength: 128, nullable: false),
                    PropiedadPlantillaId = table.Column<string>(maxLength: 128, nullable: false)
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
                name: "IX_metadatos$propiedadplantilla_PlantillaId",
                table: "metadatos$propiedadplantilla",
                column: "PlantillaId");

            migrationBuilder.CreateIndex(
                name: "IX_metadatos$tipodatopropiedadplantilla_PropiedadPlantillaId",
                table: "metadatos$tipodatopropiedadplantilla",
                column: "PropiedadPlantillaId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "metadatos$tipodatopropiedadplantilla");

            migrationBuilder.DropTable(
                name: "metadatos$propiedadplantilla");

            migrationBuilder.DropTable(
                name: "metadatos$tipodato");

            migrationBuilder.DropTable(
                name: "metadatos$plantilla");
        }
    }
}
