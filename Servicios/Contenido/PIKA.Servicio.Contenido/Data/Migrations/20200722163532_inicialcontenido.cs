using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Contenido.data.Migrations
{
    public partial class inicialcontenido : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "contenido$permiso",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Leer = table.Column<bool>(nullable: false, defaultValue: false),
                    Escribir = table.Column<bool>(nullable: false, defaultValue: false),
                    Crear = table.Column<bool>(nullable: false, defaultValue: false),
                    Eliminar = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contenido$permiso", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "contenido$tipogestores",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contenido$tipogestores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "contenido$destinatariopermiso",
                columns: table => new
                {
                    PermisoId = table.Column<string>(maxLength: 128, nullable: false),
                    DestinatarioId = table.Column<string>(maxLength: 128, nullable: false),
                    EsGrupo = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contenido$destinatariopermiso", x => new { x.PermisoId, x.DestinatarioId });
                    table.ForeignKey(
                        name: "FK_contenido$destinatariopermiso_contenido$permiso_PermisoId",
                        column: x => x.PermisoId,
                        principalTable: "contenido$permiso",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "contenido$volumen",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    TipoGestorESId = table.Column<string>(maxLength: 128, nullable: false),
                    TipoOrigenId = table.Column<string>(maxLength: 128, nullable: false),
                    OrigenId = table.Column<string>(maxLength: 128, nullable: false),
                    Activo = table.Column<bool>(nullable: false, defaultValue: true),
                    EscrituraHabilitada = table.Column<bool>(nullable: false, defaultValue: true),
                    CadenaConexion = table.Column<string>(maxLength: 2000, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false),
                    Eliminada = table.Column<bool>(nullable: false, defaultValue: false),
                    ConsecutivoVolumen = table.Column<long>(nullable: false, defaultValue: 0L),
                    CanidadPartes = table.Column<long>(nullable: false, defaultValue: 0L),
                    CanidadElementos = table.Column<long>(nullable: false, defaultValue: 0L),
                    Tamano = table.Column<long>(nullable: false, defaultValue: 0L),
                    TamanoMaximo = table.Column<long>(nullable: false, defaultValue: 0L)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contenido$volumen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_contenido$volumen_contenido$tipogestores_TipoGestorESId",
                        column: x => x.TipoGestorESId,
                        principalTable: "contenido$tipogestores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "contenido$elemento",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false),
                    Eliminada = table.Column<bool>(nullable: false, defaultValue: false),
                    TipoOrigenId = table.Column<string>(maxLength: 128, nullable: false),
                    OrigenId = table.Column<string>(maxLength: 128, nullable: false),
                    CreadorId = table.Column<string>(maxLength: 128, nullable: false),
                    FechaCreacion = table.Column<DateTime>(nullable: false),
                    VolumenId = table.Column<string>(maxLength: 128, nullable: false),
                    CarpetaId = table.Column<string>(maxLength: 128, nullable: true),
                    PermisoId = table.Column<string>(maxLength: 128, nullable: true),
                    Versionado = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contenido$elemento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_contenido$elemento_contenido$permiso_PermisoId",
                        column: x => x.PermisoId,
                        principalTable: "contenido$permiso",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_contenido$elemento_contenido$volumen_VolumenId",
                        column: x => x.VolumenId,
                        principalTable: "contenido$volumen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "contenido$pmontaje",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    TipoOrigenId = table.Column<string>(maxLength: 128, nullable: false),
                    OrigenId = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(maxLength: 500, nullable: false),
                    Eliminada = table.Column<bool>(nullable: false, defaultValue: false),
                    CreadorId = table.Column<string>(maxLength: 128, nullable: false),
                    FechaCreacion = table.Column<DateTime>(nullable: false),
                    VolumenDefaultId = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contenido$pmontaje", x => x.Id);
                    table.ForeignKey(
                        name: "FK_contenido$pmontaje_contenido$volumen_VolumenDefaultId",
                        column: x => x.VolumenDefaultId,
                        principalTable: "contenido$volumen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "contenido$versionelemento",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    ElementoId = table.Column<string>(maxLength: 128, nullable: false),
                    Activa = table.Column<bool>(nullable: false, defaultValue: false),
                    FechaCreacion = table.Column<DateTime>(nullable: false),
                    CreadorId = table.Column<string>(maxLength: 128, nullable: false),
                    Eliminada = table.Column<bool>(nullable: false, defaultValue: false)
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
                });

            migrationBuilder.CreateTable(
                name: "contenido$carpeta",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    PuntoMontajeId = table.Column<string>(nullable: true),
                    CreadorId = table.Column<string>(maxLength: 128, nullable: false),
                    FechaCreacion = table.Column<DateTime>(nullable: false),
                    Nombre = table.Column<string>(maxLength: 500, nullable: false),
                    Eliminada = table.Column<bool>(nullable: false, defaultValue: false),
                    CarpetaPadreId = table.Column<string>(maxLength: 128, nullable: true),
                    PermisoId = table.Column<string>(maxLength: 128, nullable: true),
                    EsRaiz = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contenido$carpeta", x => x.Id);
                    table.ForeignKey(
                        name: "FK_contenido$carpeta_contenido$carpeta_CarpetaPadreId",
                        column: x => x.CarpetaPadreId,
                        principalTable: "contenido$carpeta",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_contenido$carpeta_contenido$permiso_PermisoId",
                        column: x => x.PermisoId,
                        principalTable: "contenido$permiso",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_contenido$carpeta_contenido$pmontaje_PuntoMontajeId",
                        column: x => x.PuntoMontajeId,
                        principalTable: "contenido$pmontaje",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "contenido$puntomontajevolumen",
                columns: table => new
                {
                    VolumenId = table.Column<string>(maxLength: 128, nullable: false),
                    PuntoMontajeId = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contenido$puntomontajevolumen", x => new { x.VolumenId, x.PuntoMontajeId });
                    table.ForeignKey(
                        name: "FK_contenido$puntomontajevolumen_contenido$pmontaje_PuntoMontaj~",
                        column: x => x.PuntoMontajeId,
                        principalTable: "contenido$pmontaje",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_contenido$puntomontajevolumen_contenido$volumen_VolumenId",
                        column: x => x.VolumenId,
                        principalTable: "contenido$volumen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "contenido$versionpartes",
                columns: table => new
                {
                    ElementoId = table.Column<string>(maxLength: 128, nullable: false),
                    VersionId = table.Column<string>(maxLength: 128, nullable: false),
                    Id = table.Column<string>(nullable: true),
                    Indice = table.Column<int>(nullable: false, defaultValue: 0),
                    ConsecutivoVolumen = table.Column<long>(nullable: false, defaultValue: 0L),
                    TipoMime = table.Column<string>(maxLength: 50, nullable: false),
                    LongitudBytes = table.Column<int>(nullable: false, defaultValue: 0),
                    NombreOriginal = table.Column<string>(maxLength: 500, nullable: false),
                    Eliminada = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contenido$versionpartes", x => new { x.ElementoId, x.VersionId });
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
                });

            migrationBuilder.CreateIndex(
                name: "IX_contenido$carpeta_CarpetaPadreId",
                table: "contenido$carpeta",
                column: "CarpetaPadreId");

            migrationBuilder.CreateIndex(
                name: "IX_contenido$carpeta_PermisoId",
                table: "contenido$carpeta",
                column: "PermisoId");

            migrationBuilder.CreateIndex(
                name: "IX_contenido$carpeta_PuntoMontajeId",
                table: "contenido$carpeta",
                column: "PuntoMontajeId");

            migrationBuilder.CreateIndex(
                name: "IX_contenido$elemento_PermisoId",
                table: "contenido$elemento",
                column: "PermisoId");

            migrationBuilder.CreateIndex(
                name: "IX_contenido$elemento_VolumenId",
                table: "contenido$elemento",
                column: "VolumenId");

            migrationBuilder.CreateIndex(
                name: "IX_contenido$pmontaje_VolumenDefaultId",
                table: "contenido$pmontaje",
                column: "VolumenDefaultId");

            migrationBuilder.CreateIndex(
                name: "IX_contenido$puntomontajevolumen_PuntoMontajeId",
                table: "contenido$puntomontajevolumen",
                column: "PuntoMontajeId");

            migrationBuilder.CreateIndex(
                name: "IX_contenido$versionelemento_ElementoId",
                table: "contenido$versionelemento",
                column: "ElementoId");

            migrationBuilder.CreateIndex(
                name: "IX_contenido$versionpartes_VersionId",
                table: "contenido$versionpartes",
                column: "VersionId");

            migrationBuilder.CreateIndex(
                name: "IX_contenido$volumen_TipoGestorESId",
                table: "contenido$volumen",
                column: "TipoGestorESId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "contenido$carpeta");

            migrationBuilder.DropTable(
                name: "contenido$destinatariopermiso");

            migrationBuilder.DropTable(
                name: "contenido$puntomontajevolumen");

            migrationBuilder.DropTable(
                name: "contenido$versionpartes");

            migrationBuilder.DropTable(
                name: "contenido$pmontaje");

            migrationBuilder.DropTable(
                name: "contenido$versionelemento");

            migrationBuilder.DropTable(
                name: "contenido$elemento");

            migrationBuilder.DropTable(
                name: "contenido$permiso");

            migrationBuilder.DropTable(
                name: "contenido$volumen");

            migrationBuilder.DropTable(
                name: "contenido$tipogestores");
        }
    }
}
