using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Contenido.data.Migrations
{
    public partial class GestoresRenombramientoCampo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Contraena",
                table: "contenido$gestorsmb");

            migrationBuilder.DropColumn(
                name: "Contraena",
                table: "contenido$gestorazure");

            migrationBuilder.AddColumn<string>(
                name: "Contrasena",
                table: "contenido$gestorsmb",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Contrasena",
                table: "contenido$gestorazure",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Contrasena",
                table: "contenido$gestorsmb");

            migrationBuilder.DropColumn(
                name: "Contrasena",
                table: "contenido$gestorazure");

            migrationBuilder.AddColumn<string>(
                name: "Contraena",
                table: "contenido$gestorsmb",
                type: "varchar(200) CHARACTER SET utf8mb4",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Contraena",
                table: "contenido$gestorazure",
                type: "varchar(200) CHARACTER SET utf8mb4",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }
    }
}
