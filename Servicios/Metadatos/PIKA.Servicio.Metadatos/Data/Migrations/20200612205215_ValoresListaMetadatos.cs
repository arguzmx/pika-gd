using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Metadatos.Data.Migrations
{
    public partial class ValoresListaMetadatos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "metadatos$valorespropiedad",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Texto = table.Column<string>(maxLength: 200, nullable: false),
                    Indice = table.Column<int>(nullable: false, defaultValue: 0),
                    PropiedadId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_metadatos$valorespropiedad", x => x.Id);
                    table.ForeignKey(
                        name: "FK_metadatos$valorespropiedad_metadatos$propiedadplantilla_Prop~",
                        column: x => x.PropiedadId,
                        principalTable: "metadatos$propiedadplantilla",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_metadatos$valorespropiedad_PropiedadId",
                table: "metadatos$valorespropiedad",
                column: "PropiedadId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "metadatos$valorespropiedad");
        }
    }
}
