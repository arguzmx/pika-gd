using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Contenido.Data.migrations
{
    public partial class LFConfigMoreProps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ConvertirTiff",
                table: "contenido$gestorlf",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "FormatoConversion",
                table: "contenido$gestorlf",
                maxLength: 10,
                nullable: false,
                defaultValue: "JPG");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConvertirTiff",
                table: "contenido$gestorlf");

            migrationBuilder.DropColumn(
                name: "FormatoConversion",
                table: "contenido$gestorlf");
        }
    }
}
