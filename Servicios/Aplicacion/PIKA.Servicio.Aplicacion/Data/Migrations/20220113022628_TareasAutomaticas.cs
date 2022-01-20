using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.AplicacionPlugin.Data.Migrations
{
    public partial class TareasAutomaticas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaInstalacion",
                table: "aplicacion$plugininstalado",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2020, 4, 27, 20, 37, 43, 722, DateTimeKind.Local).AddTicks(2141));

            migrationBuilder.CreateTable(
                name: "aplicacion$autotarea",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CodigoError = table.Column<string>(maxLength: 250, nullable: true),
                    Exito = table.Column<bool>(nullable: true),
                    UltimaEjecucion = table.Column<DateTime>(nullable: true),
                    ProximaEjecucion = table.Column<DateTime>(nullable: true),
                    Duracion = table.Column<int>(nullable: true),
                    Periodo = table.Column<int>(nullable: false),
                    FechaHoraEjecucion = table.Column<DateTime>(nullable: true),
                    Intervalo = table.Column<int>(nullable: true),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false),
                    TipoOrigenId = table.Column<string>(maxLength: 128, nullable: true),
                    OrigenId = table.Column<string>(maxLength: 128, nullable: true),
                    Ensamblado = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_aplicacion$autotarea", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "aplicacion$bitacoratarea",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    BitacoraTareaId = table.Column<string>(maxLength: 128, nullable: true),
                    UltimaEjecucion = table.Column<DateTime>(nullable: false),
                    Duracion = table.Column<int>(nullable: false),
                    Exito = table.Column<bool>(nullable: false),
                    CodigoError = table.Column<string>(maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_aplicacion$bitacoratarea", x => x.Id);
                    table.ForeignKey(
                        name: "FK_aplicacion$bitacoratarea_aplicacion$autotarea_BitacoraTareaId",
                        column: x => x.BitacoraTareaId,
                        principalTable: "aplicacion$autotarea",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_aplicacion$bitacoratarea_BitacoraTareaId",
                table: "aplicacion$bitacoratarea",
                column: "BitacoraTareaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "aplicacion$bitacoratarea");

            migrationBuilder.DropTable(
                name: "aplicacion$autotarea");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaInstalacion",
                table: "aplicacion$plugininstalado",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2020, 4, 27, 20, 37, 43, 722, DateTimeKind.Local).AddTicks(2141),
                oldClrType: typeof(DateTime),
                oldNullable: true);
        }
    }
}
