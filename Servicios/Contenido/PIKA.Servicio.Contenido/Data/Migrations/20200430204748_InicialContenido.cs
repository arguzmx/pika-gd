using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Contenido.Data.Migrations
{
    public partial class InicialContenido : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Contenido$tipogestores",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false),
                    Volumenid = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contenido$tipogestores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Contenido$volumen",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    TipoOrigenId = table.Column<string>(maxLength: 128, nullable: false),
                    OrigenId = table.Column<string>(maxLength: 128, nullable: false),
                    Activo = table.Column<bool>(nullable: false, defaultValue: false),
                    EscrituraHabilitada = table.Column<bool>(nullable: false, defaultValue: false),
                    TipoGestorESId = table.Column<string>(maxLength: 128, nullable: false),
                    CadenaConexion = table.Column<string>(maxLength: 2000, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false),
                    Eliminada = table.Column<bool>(nullable: false, defaultValue: false),
                    ConsecutivoVolumen = table.Column<long>(nullable: false, defaultValue: 0L),
                    CanidadPartes = table.Column<long>(nullable: false, defaultValue: 0L),
                    Tamano = table.Column<long>(nullable: false, defaultValue: 0L),
                    GestorESId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contenido$volumen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contenido$volumen_Contenido$tipogestores_GestorESId",
                        column: x => x.GestorESId,
                        principalTable: "Contenido$tipogestores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Contenido$volumen_Contenido$tipogestores_TipoGestorESId",
                        column: x => x.TipoGestorESId,
                        principalTable: "Contenido$tipogestores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Contenido$elemento",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false),
                    Eliminada = table.Column<bool>(nullable: false, defaultValue: false),
                    TipoOrigenId = table.Column<string>(maxLength: 128, nullable: false),
                    OrigenId = table.Column<string>(maxLength: 128, nullable: false),
                    CreadorId = table.Column<string>(maxLength: 128, nullable: false),
                    FechaCreacion = table.Column<DateTime>(nullable: false, defaultValue: new DateTime(2020, 4, 30, 15, 47, 47, 904, DateTimeKind.Local).AddTicks(954)),
                    VolumenId = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contenido$elemento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contenido$elemento_Contenido$volumen_VolumenId",
                        column: x => x.VolumenId,
                        principalTable: "Contenido$volumen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Contenido$versionelemento",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    ElementoId = table.Column<string>(maxLength: 128, nullable: false),
                    Activa = table.Column<bool>(nullable: false, defaultValue: false),
                    FechaCreacion = table.Column<DateTime>(nullable: false, defaultValue: new DateTime(2020, 4, 30, 15, 47, 47, 950, DateTimeKind.Local).AddTicks(7517)),
                    FechaActualizacion = table.Column<DateTime>(nullable: false, defaultValue: new DateTime(2020, 4, 30, 15, 47, 47, 950, DateTimeKind.Local).AddTicks(8286))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contenido$versionelemento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contenido$versionelemento_Contenido$elemento_ElementoId",
                        column: x => x.ElementoId,
                        principalTable: "Contenido$elemento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Contenido$parte",
                columns: table => new
                {
                    ElementoId = table.Column<string>(maxLength: 128, nullable: false),
                    VersionId = table.Column<string>(maxLength: 128, nullable: false),
                    Indice = table.Column<int>(nullable: false, defaultValue: 0),
                    ConsecutivoVolumen = table.Column<long>(nullable: false, defaultValue: 0L),
                    TipoMime = table.Column<string>(maxLength: 50, nullable: false),
                    LongitudBytes = table.Column<int>(nullable: false, defaultValue: 0),
                    NombreOriginal = table.Column<string>(maxLength: 200, nullable: false),
                    Eliminada = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contenido$parte", x => new { x.ElementoId, x.VersionId });
                    table.ForeignKey(
                        name: "FK_Contenido$parte_Contenido$elemento_ElementoId",
                        column: x => x.ElementoId,
                        principalTable: "Contenido$elemento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Contenido$parte_Contenido$versionelemento_VersionId",
                        column: x => x.VersionId,
                        principalTable: "Contenido$versionelemento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contenido$elemento_VolumenId",
                table: "Contenido$elemento",
                column: "VolumenId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contenido$parte_ElementoId",
                table: "Contenido$parte",
                column: "ElementoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contenido$parte_VersionId",
                table: "Contenido$parte",
                column: "VersionId");

            migrationBuilder.CreateIndex(
                name: "IX_Contenido$versionelemento_ElementoId",
                table: "Contenido$versionelemento",
                column: "ElementoId");

            migrationBuilder.CreateIndex(
                name: "IX_Contenido$volumen_GestorESId",
                table: "Contenido$volumen",
                column: "GestorESId");

            migrationBuilder.CreateIndex(
                name: "IX_Contenido$volumen_TipoGestorESId",
                table: "Contenido$volumen",
                column: "TipoGestorESId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contenido$parte");

            migrationBuilder.DropTable(
                name: "Contenido$versionelemento");

            migrationBuilder.DropTable(
                name: "Contenido$elemento");

            migrationBuilder.DropTable(
                name: "Contenido$volumen");

            migrationBuilder.DropTable(
                name: "Contenido$tipogestores");
        }
    }
}
