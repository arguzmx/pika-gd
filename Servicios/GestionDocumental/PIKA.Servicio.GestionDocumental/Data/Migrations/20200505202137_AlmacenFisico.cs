using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.GestionDocumental.Data.Migrations
{
    public partial class AlmacenFisico : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "gd$almacen",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false),
                    Clave = table.Column<string>(maxLength: 200, nullable: false),
                    ArchivoId = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$almacen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_gd$almacen_gd$archivo_ArchivoId",
                        column: x => x.ArchivoId,
                        principalTable: "gd$archivo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "gd$estantes",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    AlmacenArchivoId = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false),
                    CodigoOptico = table.Column<string>(maxLength: 2048, nullable: true),
                    CodigoElectronico = table.Column<string>(maxLength: 2048, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$estantes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_gd$estantes_gd$almacen_AlmacenArchivoId",
                        column: x => x.AlmacenArchivoId,
                        principalTable: "gd$almacen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "gd$espacio_estante",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false),
                    CodigoOptico = table.Column<string>(maxLength: 2048, nullable: true),
                    CodigoElectronico = table.Column<string>(maxLength: 2048, nullable: true),
                    EstanteId = table.Column<string>(maxLength: 128, nullable: false),
                    Posicion = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$espacio_estante", x => x.Id);
                    table.ForeignKey(
                        name: "FK_gd$espacio_estante_gd$estantes_EstanteId",
                        column: x => x.EstanteId,
                        principalTable: "gd$estantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_gd$almacen_ArchivoId",
                table: "gd$almacen",
                column: "ArchivoId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$espacio_estante_EstanteId",
                table: "gd$espacio_estante",
                column: "EstanteId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$estantes_AlmacenArchivoId",
                table: "gd$estantes",
                column: "AlmacenArchivoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "gd$espacio_estante");

            migrationBuilder.DropTable(
                name: "gd$estantes");

            migrationBuilder.DropTable(
                name: "gd$almacen");
        }
    }
}
