using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.GestionDocumental.Data.Migrations
{
    public partial class EntradaCalsificacionDatos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AbreCon",
                table: "gd$entradaclasificacion",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CierraCon",
                table: "gd$entradaclasificacion",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Contiene",
                table: "gd$entradaclasificacion",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InstruccionFinal",
                table: "gd$entradaclasificacion",
                maxLength: 500,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AbreCon",
                table: "gd$entradaclasificacion");

            migrationBuilder.DropColumn(
                name: "CierraCon",
                table: "gd$entradaclasificacion");

            migrationBuilder.DropColumn(
                name: "Contiene",
                table: "gd$entradaclasificacion");

            migrationBuilder.DropColumn(
                name: "InstruccionFinal",
                table: "gd$entradaclasificacion");
        }
    }
}
