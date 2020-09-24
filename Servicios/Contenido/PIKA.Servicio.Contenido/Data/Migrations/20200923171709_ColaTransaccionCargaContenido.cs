using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Contenido.data.Migrations
{
    public partial class ColaTransaccionCargaContenido : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AlterColumn<long>(
            //    name: "LongitudBytes",
            //    table: "contenido$versionpartes",
            //    nullable: false,
            //    oldClrType: typeof(int),
            //    oldType: "int");

            migrationBuilder.CreateTable(
                name: "contenido$ElementoTransaccionContenido",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    TransaccionId = table.Column<string>(maxLength: 128, nullable: false),
                    VolumenId = table.Column<string>(maxLength: 128, nullable: false),
                    ElementoId = table.Column<string>(maxLength: 128, nullable: false),
                    PuntoMontajeId = table.Column<string>(maxLength: 128, nullable: false),
                    Indice = table.Column<int>(nullable: false),
                    NombreOriginal = table.Column<string>(maxLength: 200, nullable: false),
                    TamanoBytes = table.Column<long>(nullable: false),
                    FechaCarga = table.Column<DateTime>(nullable: false),
                    FechaProceso = table.Column<DateTime>(nullable: true),
                    Procesado = table.Column<bool>(nullable: false, defaultValue: false),
                    Error = table.Column<bool>(nullable: false, defaultValue: false),
                    Info = table.Column<string>(maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contenido$ElementoTransaccionContenido", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_contenido$ElementoTransaccionContenido_TransaccionId",
                table: "contenido$ElementoTransaccionContenido",
                column: "TransaccionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "contenido$ElementoTransaccionContenido");

            //migrationBuilder.AlterColumn<int>(
            //    name: "LongitudBytes",
            //    table: "contenido$versionpartes",
            //    type: "int",
            //    nullable: false,
            //    oldClrType: typeof(long));
        }
    }
}
