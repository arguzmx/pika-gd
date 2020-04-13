using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Metadatos.Data.Migrations
{
    public partial class AtributosMetadatos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "metadatos$atributometadato",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    PropiedadId = table.Column<string>(maxLength: 128, nullable: false),
                    PropiedadPlantillaId1 = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_metadatos$atributometadato", x => x.Id);
                    table.ForeignKey(
                        name: "FK_metadatos$atributometadato_metadatos$propiedadplantilla_Id",
                        column: x => x.Id,
                        principalTable: "metadatos$propiedadplantilla",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_metadatos$atributometadato_metadatos$propiedadplantilla_Prop~",
                        column: x => x.PropiedadPlantillaId1,
                        principalTable: "metadatos$propiedadplantilla",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "metadatos$validadornumero",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    PropiedadId = table.Column<string>(maxLength: 128, nullable: true),
                    min = table.Column<float>(nullable: false),
                    max = table.Column<float>(nullable: false),
                    valordefault = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_metadatos$validadornumero", x => x.Id);
                    table.ForeignKey(
                        name: "FK_metadatos$validadornumero_metadatos$propiedadplantilla_Id",
                        column: x => x.Id,
                        principalTable: "metadatos$propiedadplantilla",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "metadatos$validadortexto",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    PropiedadId = table.Column<string>(maxLength: 128, nullable: true),
                    longmin = table.Column<int>(nullable: false),
                    longmax = table.Column<int>(nullable: false),
                    valordefaulr = table.Column<string>(maxLength: 128, nullable: false),
                    regexp = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_metadatos$validadortexto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_metadatos$validadortexto_metadatos$propiedadplantilla_Id",
                        column: x => x.Id,
                        principalTable: "metadatos$propiedadplantilla",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_metadatos$atributometadato_PropiedadPlantillaId1",
                table: "metadatos$atributometadato",
                column: "PropiedadPlantillaId1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "metadatos$atributometadato");

            migrationBuilder.DropTable(
                name: "metadatos$validadornumero");

            migrationBuilder.DropTable(
                name: "metadatos$validadortexto");
        }
    }
}
