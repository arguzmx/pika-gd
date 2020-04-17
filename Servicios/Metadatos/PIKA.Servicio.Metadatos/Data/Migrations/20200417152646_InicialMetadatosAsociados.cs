using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Metadatos.Data.Migrations
{
    public partial class InicialMetadatosAsociados : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "metadatos$tipoalmacenmetadatos",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false),
                    AsociacionPlantillaid = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_metadatos$tipoalmacenmetadatos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "metadatos$asociacionplantilla",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    TipoOrigenId = table.Column<string>(maxLength: 128, nullable: false),
                    OrigenId = table.Column<string>(maxLength: 128, nullable: false),
                    PlantillaId = table.Column<string>(maxLength: 128, nullable: false),
                    IdentificadorAlmacenamiento = table.Column<string>(maxLength: 128, nullable: false),
                    TipoAlmacenMetadatosId = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_metadatos$asociacionplantilla", x => x.Id);
                    table.ForeignKey(
                        name: "FK_metadatos$asociacionplantilla_metadatos$plantilla_Id",
                        column: x => x.Id,
                        principalTable: "metadatos$plantilla",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_metadatos$asociacionplantilla_metadatos$tipoalmacenmetadatos~",
                        column: x => x.Id,
                        principalTable: "metadatos$tipoalmacenmetadatos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "metadatos$asociacionplantilla");

            migrationBuilder.DropTable(
                name: "metadatos$tipoalmacenmetadatos");
        }
    }
}
