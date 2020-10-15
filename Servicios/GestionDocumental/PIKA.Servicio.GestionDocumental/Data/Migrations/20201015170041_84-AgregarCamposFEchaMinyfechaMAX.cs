using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.GestionDocumental.Data.Migrations
{
    public partial class _84AgregarCamposFEchaMinyfechaMAX : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaMaxCierre",
                table: "gd$estadisticaclasificacionacervo",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaMinApertura",
                table: "gd$estadisticaclasificacionacervo",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaMaxCierre",
                table: "gd$estadisticaclasificacionacervo");

            migrationBuilder.DropColumn(
                name: "FechaMinApertura",
                table: "gd$estadisticaclasificacionacervo");
        }
    }
}
