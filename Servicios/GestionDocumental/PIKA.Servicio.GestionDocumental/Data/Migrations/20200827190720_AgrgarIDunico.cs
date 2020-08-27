using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.GestionDocumental.Data.Migrations
{
    public partial class AgrgarIDunico : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EsElectronio",
                table: "gd$activo");

            migrationBuilder.AddColumn<bool>(
                name: "EsElectronico",
                table: "gd$activo",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "IDunico",
                table: "gd$activo",
                maxLength: 250,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EsElectronico",
                table: "gd$activo");

            migrationBuilder.DropColumn(
                name: "IDunico",
                table: "gd$activo");

            migrationBuilder.AddColumn<bool>(
                name: "EsElectronio",
                table: "gd$activo",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }
    }
}
