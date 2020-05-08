using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.GestionDocumental.Data.Migrations
{
    public partial class Archivo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "gd$faseciclovital",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$faseciclovital", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "gd$tipoarchivo",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false),
                    FaseCicloVitalId = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$tipoarchivo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_gd$tipoarchivo_gd$faseciclovital_FaseCicloVitalId",
                        column: x => x.FaseCicloVitalId,
                        principalTable: "gd$faseciclovital",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "gd$archivo",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false),
                    Eliminada = table.Column<bool>(nullable: false, defaultValue: false),
                    TipoOrigenId = table.Column<string>(maxLength: 128, nullable: false),
                    OrigenId = table.Column<string>(maxLength: 128, nullable: false),
                    TipoArchivoId = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$archivo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_gd$archivo_gd$tipoarchivo_TipoArchivoId",
                        column: x => x.TipoArchivoId,
                        principalTable: "gd$tipoarchivo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_gd$archivo_TipoArchivoId",
                table: "gd$archivo",
                column: "TipoArchivoId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$tipoarchivo_FaseCicloVitalId",
                table: "gd$tipoarchivo",
                column: "FaseCicloVitalId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "gd$archivo");

            migrationBuilder.DropTable(
                name: "gd$tipoarchivo");

            migrationBuilder.DropTable(
                name: "gd$faseciclovital");
        }
    }
}
