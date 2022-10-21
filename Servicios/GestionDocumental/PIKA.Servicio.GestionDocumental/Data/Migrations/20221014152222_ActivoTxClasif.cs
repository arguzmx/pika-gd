using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.GestionDocumental.data.migrations
{
    public partial class ActivoTxClasif : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CuadroClasificacionId",
                table: "gd$activotransferencia",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EntradaClasificacionId",
                table: "gd$activotransferencia",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaRetencion",
                table: "gd$activotransferencia",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CuadroClasificacionId",
                table: "gd$activotransferencia");

            migrationBuilder.DropColumn(
                name: "EntradaClasificacionId",
                table: "gd$activotransferencia");

            migrationBuilder.DropColumn(
                name: "FechaRetencion",
                table: "gd$activotransferencia");
        }
    }
}
