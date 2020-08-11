using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.GestionDocumental.Data.Migrations
{
    public partial class DropAmpliacion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "gd$ampliacion");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "gd$ampliacion",
                columns: table => new
                {
                    ActivoId = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false),
                    Vigente = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    Anos = table.Column<int>(type: "int", nullable: true),
                    Dias = table.Column<int>(type: "int", nullable: true),
                    FechaFija = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    Fin = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    FundamentoLegal = table.Column<string>(type: "varchar(2000) CHARACTER SET utf8mb4", maxLength: 2000, nullable: false),
                    Inicio = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Meses = table.Column<int>(type: "int", nullable: true),
                    TipoAmpliacionId = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false)
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
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_gd$ampliacion_TipoAmpliacionId",
                table: "gd$ampliacion",
                column: "TipoAmpliacionId");
        }
    }
}
