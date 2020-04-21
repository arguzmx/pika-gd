using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.GestionDocumental.Data.Migrations
{
    public partial class ActivosGestionDocumental : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "gd$activo",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    TipoOrigenId = table.Column<string>(maxLength: 128, nullable: false),
                    OrigenId = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false),
                    Asunto = table.Column<string>(maxLength: 2048, nullable: true),
                    FechaApertura = table.Column<DateTime>(nullable: false),
                    FechaCierre = table.Column<DateTime>(nullable: true),
                    EsElectronio = table.Column<bool>(nullable: false, defaultValue: true),
                    CodigoOptico = table.Column<string>(maxLength: 1024, nullable: true),
                    CodigoElectronico = table.Column<string>(maxLength: 1024, nullable: true),
                    ElementoClasificacionId = table.Column<string>(nullable: true),
                    ArchivoId = table.Column<string>(nullable: true),
                    EnPrestamo = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$activo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_gd$activo_gd$archivo_ArchivoId",
                        column: x => x.ArchivoId,
                        principalTable: "gd$archivo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_gd$activo_gd$elementoclasificacion_ElementoClasificacionId",
                        column: x => x.ElementoClasificacionId,
                        principalTable: "gd$elementoclasificacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "gd$asunto",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    ActivoId = table.Column<string>(nullable: true),
                    Contenido = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$asunto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_gd$asunto_gd$activo_ActivoId",
                        column: x => x.ActivoId,
                        principalTable: "gd$activo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "gd$historialarchivoactivo",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    ActivoId = table.Column<string>(maxLength: 128, nullable: false),
                    ArchivoId = table.Column<string>(maxLength: 128, nullable: false),
                    FechaIngreso = table.Column<DateTime>(nullable: false),
                    FechaEgreso = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$historialarchivoactivo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_gd$historialarchivoactivo_gd$activo_ActivoId",
                        column: x => x.ActivoId,
                        principalTable: "gd$activo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_gd$historialarchivoactivo_gd$archivo_ArchivoId",
                        column: x => x.ArchivoId,
                        principalTable: "gd$archivo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_gd$activo_ArchivoId",
                table: "gd$activo",
                column: "ArchivoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_gd$activo_ElementoClasificacionId",
                table: "gd$activo",
                column: "ElementoClasificacionId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$asunto_ActivoId",
                table: "gd$asunto",
                column: "ActivoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_gd$historialarchivoactivo_ActivoId",
                table: "gd$historialarchivoactivo",
                column: "ActivoId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$historialarchivoactivo_ArchivoId",
                table: "gd$historialarchivoactivo",
                column: "ArchivoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "gd$asunto");

            migrationBuilder.DropTable(
                name: "gd$historialarchivoactivo");

            migrationBuilder.DropTable(
                name: "gd$activo");
        }
    }
}
