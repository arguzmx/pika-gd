using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Seguridad.Data.Migrations
{
    public partial class PropiedadEliminadaInactiva : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Eliminada",
                table: "seguridad$usuarioprops",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Inactiva",
                table: "seguridad$usuarioprops",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Eliminada",
                table: "aspnetusers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Inactiva",
                table: "aspnetusers",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Eliminada",
                table: "seguridad$usuarioprops");

            migrationBuilder.DropColumn(
                name: "Inactiva",
                table: "seguridad$usuarioprops");

            migrationBuilder.DropColumn(
                name: "Eliminada",
                table: "aspnetusers");

            migrationBuilder.DropColumn(
                name: "Inactiva",
                table: "aspnetusers");
        }
    }
}
