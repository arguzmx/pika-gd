using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.AplicacionPlugin.Data.Migrations
{
    public partial class InicialAplicacionPlugin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "aplicacion$plugin",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false),
                    Gratuito = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_aplicacion$plugin", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "aplicacion$versionplugin",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    PluginId = table.Column<string>(maxLength: 128, nullable: false),
                    Version = table.Column<string>(maxLength: 10, nullable: false),
                    URL = table.Column<string>(maxLength: 128, nullable: false),
                    RequiereConfiguracion = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_aplicacion$versionplugin", x => x.Id);
                    table.ForeignKey(
                        name: "FK_aplicacion$versionplugin_aplicacion$plugin_PluginId",
                        column: x => x.PluginId,
                        principalTable: "aplicacion$plugin",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "aplicacion$plugininstalado",
                columns: table => new
                {
                    PLuginId = table.Column<string>(maxLength: 128, nullable: false),
                    VersionPLuginId = table.Column<string>(maxLength: 128, nullable: false),
                    Activo = table.Column<bool>(nullable: false, defaultValue: false),
                    FechaInstalacion = table.Column<DateTime>(nullable: false, defaultValue: new DateTime(2020, 4, 27, 16, 36, 21, 40, DateTimeKind.Local).AddTicks(2788))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_aplicacion$plugininstalado", x => new { x.PLuginId, x.VersionPLuginId });
                    table.ForeignKey(
                        name: "FK_aplicacion$plugininstalado_aplicacion$plugin_PLuginId",
                        column: x => x.PLuginId,
                        principalTable: "aplicacion$plugin",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_aplicacion$plugininstalado_aplicacion$versionplugin_VersionP~",
                        column: x => x.VersionPLuginId,
                        principalTable: "aplicacion$versionplugin",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_aplicacion$plugininstalado_VersionPLuginId",
                table: "aplicacion$plugininstalado",
                column: "VersionPLuginId");

            migrationBuilder.CreateIndex(
                name: "IX_aplicacion$versionplugin_PluginId",
                table: "aplicacion$versionplugin",
                column: "PluginId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "aplicacion$plugininstalado");

            migrationBuilder.DropTable(
                name: "aplicacion$versionplugin");

            migrationBuilder.DropTable(
                name: "aplicacion$plugin");
        }
    }
}
