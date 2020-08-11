using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.GestionDocumental.Data.Migrations
{
    public partial class CreaNuevaAmpliacion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "gd$ampliacion",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    ActivoId = table.Column<string>(maxLength: 128, nullable: false),
                    Vigente = table.Column<bool>(nullable: false, defaultValue: false),
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
                    table.PrimaryKey("PK_gd$ampliacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_gd$ampliacion_gd$activo_ActivoId",
                        column: x => x.ActivoId,
                        principalTable: "gd$activo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_gd$ampliacion_gd$tipoampliacion_TipoAmpliacionId",
                        column: x => x.TipoAmpliacionId,
                        principalTable: "gd$tipoampliacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_gd$ampliacion_ActivoId",
                table: "gd$ampliacion",
                column: "ActivoId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$ampliacion_TipoAmpliacionId",
                table: "gd$ampliacion",
                column: "TipoAmpliacionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "gd$ampliacion");
        }
    }
}
