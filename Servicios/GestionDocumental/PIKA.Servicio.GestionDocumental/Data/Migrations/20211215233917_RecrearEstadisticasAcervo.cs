using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.GestionDocumental.Data.Migrations
{
    public partial class RecrearEstadisticasAcervo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "gd$estadisticaclasificacionacervo",
                columns: table => new
                {
                    ArchivoId = table.Column<string>(maxLength: 128, nullable: false),
                    UnidadAdministrativaArchivoId = table.Column<string>(maxLength: 128, nullable: false),
                    CuadroClasificacionId = table.Column<string>(maxLength: 128, nullable: false),
                    EntradaClasificacionId = table.Column<string>(maxLength: 128, nullable: false),
                    ConteoActivos = table.Column<int>(nullable: false),
                    ConteoActivosEliminados = table.Column<int>(nullable: false),
                    FechaMinApertura = table.Column<DateTime>(nullable: true),
                    FechaMaxCierre = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$estadisticaclasificacionacervo", x => new { x.ArchivoId, x.CuadroClasificacionId, x.UnidadAdministrativaArchivoId, x.EntradaClasificacionId });
                    table.ForeignKey(
                        name: "FK_gd$estadisticaclasificacionacervo_gd$archivo_ArchivoId",
                        column: x => x.ArchivoId,
                        principalTable: "gd$archivo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_gd$estadisticaclasificacionacervo_gd$cuadroclasificacion_Cua~",
                        column: x => x.CuadroClasificacionId,
                        principalTable: "gd$cuadroclasificacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_gd$estadisticaclasificacionacervo_gd$entradaclasificacion_En~",
                        column: x => x.EntradaClasificacionId,
                        principalTable: "gd$entradaclasificacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_gd$estadisticaclasificacionacervo_gd$unidadadministrativaarc~",
                        column: x => x.UnidadAdministrativaArchivoId,
                        principalTable: "gd$unidadadministrativaarchivo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_gd$estadisticaclasificacionacervo_CuadroClasificacionId",
                table: "gd$estadisticaclasificacionacervo",
                column: "CuadroClasificacionId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$estadisticaclasificacionacervo_EntradaClasificacionId",
                table: "gd$estadisticaclasificacionacervo",
                column: "EntradaClasificacionId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$estadisticaclasificacionacervo_UnidadAdministrativaArchiv~",
                table: "gd$estadisticaclasificacionacervo",
                column: "UnidadAdministrativaArchivoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "gd$estadisticaclasificacionacervo");
        }
    }
}
