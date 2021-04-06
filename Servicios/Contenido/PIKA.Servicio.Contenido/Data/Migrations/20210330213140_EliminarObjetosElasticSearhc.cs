using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Contenido.Data.Migrations
{
    public partial class EliminarObjetosElasticSearhc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "contenido$versionpartes");

            migrationBuilder.DropTable(
                name: "contenido$versionelemento");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "contenido$versionelemento",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false),
                    Activa = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ConteoPartes = table.Column<int>(type: "int", nullable: false),
                    CreadorId = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false),
                    ElementoId = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false),
                    Eliminada = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    MaxIndicePartes = table.Column<int>(type: "int", nullable: false),
                    TamanoBytes = table.Column<long>(type: "bigint", nullable: false),
                    VolumenId = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contenido$versionelemento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_contenido$versionelemento_contenido$elemento_ElementoId",
                        column: x => x.ElementoId,
                        principalTable: "contenido$elemento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_contenido$versionelemento_contenido$volumen_VolumenId",
                        column: x => x.VolumenId,
                        principalTable: "contenido$volumen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "contenido$versionpartes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false),
                    ConsecutivoVolumen = table.Column<long>(type: "bigint", nullable: false),
                    ElementoId = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false),
                    Eliminada = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    EsAudio = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    EsImagen = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    EsPDF = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    EsVideo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Extension = table.Column<string>(type: "varchar(100) CHARACTER SET utf8mb4", maxLength: 100, nullable: false),
                    Indexada = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Indice = table.Column<int>(type: "int", nullable: false),
                    LongitudBytes = table.Column<long>(type: "bigint", nullable: false),
                    NombreOriginal = table.Column<string>(type: "varchar(500) CHARACTER SET utf8mb4", maxLength: 500, nullable: false),
                    TieneMiniatura = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TipoMime = table.Column<string>(type: "varchar(100) CHARACTER SET utf8mb4", maxLength: 100, nullable: true),
                    VersionId = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false),
                    VolumenId = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contenido$versionpartes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_contenido$versionpartes_contenido$elemento_ElementoId",
                        column: x => x.ElementoId,
                        principalTable: "contenido$elemento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_contenido$versionpartes_contenido$versionelemento_VersionId",
                        column: x => x.VersionId,
                        principalTable: "contenido$versionelemento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_contenido$versionpartes_contenido$volumen_VolumenId",
                        column: x => x.VolumenId,
                        principalTable: "contenido$volumen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_contenido$versionelemento_ElementoId",
                table: "contenido$versionelemento",
                column: "ElementoId");

            migrationBuilder.CreateIndex(
                name: "IX_contenido$versionelemento_VolumenId",
                table: "contenido$versionelemento",
                column: "VolumenId");

            migrationBuilder.CreateIndex(
                name: "IX_contenido$versionpartes_ElementoId",
                table: "contenido$versionpartes",
                column: "ElementoId");

            migrationBuilder.CreateIndex(
                name: "IX_contenido$versionpartes_Indexada",
                table: "contenido$versionpartes",
                column: "Indexada");

            migrationBuilder.CreateIndex(
                name: "IX_contenido$versionpartes_VersionId",
                table: "contenido$versionpartes",
                column: "VersionId");

            migrationBuilder.CreateIndex(
                name: "IX_contenido$versionpartes_VolumenId",
                table: "contenido$versionpartes",
                column: "VolumenId");
        }
    }
}
