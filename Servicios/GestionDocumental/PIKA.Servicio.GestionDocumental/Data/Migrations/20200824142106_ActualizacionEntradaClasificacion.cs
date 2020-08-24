using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.GestionDocumental.Data.Migrations
{
    public partial class ActualizacionEntradaClasificacion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MesesVigenciConcentracion",
                table: "gd$entradaclasificacion");

            migrationBuilder.DropColumn(
                name: "MesesVigenciHistorico",
                table: "gd$entradaclasificacion");

            migrationBuilder.DropColumn(
                name: "MesesVigenciTramite",
                table: "gd$entradaclasificacion");

            migrationBuilder.AddColumn<int>(
                name: "VigenciaConcentracion",
                table: "gd$entradaclasificacion",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VigenciaTramite",
                table: "gd$entradaclasificacion",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VigenciaConcentracion",
                table: "gd$entradaclasificacion");

            migrationBuilder.DropColumn(
                name: "VigenciaTramite",
                table: "gd$entradaclasificacion");

            migrationBuilder.AddColumn<int>(
                name: "MesesVigenciConcentracion",
                table: "gd$entradaclasificacion",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MesesVigenciHistorico",
                table: "gd$entradaclasificacion",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MesesVigenciTramite",
                table: "gd$entradaclasificacion",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
