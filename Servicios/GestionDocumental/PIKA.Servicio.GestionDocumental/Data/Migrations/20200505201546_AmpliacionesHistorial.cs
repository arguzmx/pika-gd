using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.GestionDocumental.Data.Migrations
{
    public partial class AmpliacionesHistorial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Eliminada",
                table: "gd$activo",
                nullable: false,
                defaultValue: false);

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

            migrationBuilder.CreateTable(
                name: "gd$tipoampliacion",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$tipoampliacion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "gd$ampliacion",
                columns: table => new
                {
                    ActivoId = table.Column<string>(maxLength: 128, nullable: false),
                    Vigente = table.Column<bool>(nullable: false, defaultValue: false),
                    Id = table.Column<string>(nullable: true),
                    TipoAmpliacionId = table.Column<string>(maxLength: 128, nullable: false),
                    FechaFija = table.Column<bool>(nullable: false, defaultValue: false),
                    FundamentoLegal = table.Column<string>(maxLength: 2000, nullable: false),
                    Inicio = table.Column<DateTime>(nullable: true),
                    Fin = table.Column<DateTime>(nullable: true),
                    Anos = table.Column<int>(nullable: true),
                    Meses = table.Column<int>(nullable: true),
                    Dias = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$ampliacion", x => new { x.ActivoId, x.Vigente });
                    table.ForeignKey(
                        name: "FK_gd$ampliacion_gd$activo_ActivoId",
                        column: x => x.ActivoId,
                        principalTable: "gd$activo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_gd$ampliacion_gd$tipoampliacion_TipoAmpliacionId",
                        column: x => x.TipoAmpliacionId,
                        principalTable: "gd$tipoampliacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_gd$ampliacion_TipoAmpliacionId",
                table: "gd$ampliacion",
                column: "TipoAmpliacionId");

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
                name: "gd$ampliacion");

            migrationBuilder.DropTable(
                name: "gd$historialarchivoactivo");

            migrationBuilder.DropTable(
                name: "gd$tipoampliacion");

            migrationBuilder.DropColumn(
                name: "Eliminada",
                table: "gd$activo");
        }
    }
}
