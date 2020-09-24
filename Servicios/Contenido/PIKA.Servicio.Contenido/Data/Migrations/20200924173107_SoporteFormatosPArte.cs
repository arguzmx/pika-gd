using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Contenido.data.Migrations
{
    public partial class SoporteFormatosPArte : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TipoMime",
                table: "contenido$versionpartes",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50) CHARACTER SET utf8mb4",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<bool>(
                name: "EsAudio",
                table: "contenido$versionpartes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EsPDF",
                table: "contenido$versionpartes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EsVideo",
                table: "contenido$versionpartes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Extension",
                table: "contenido$versionpartes",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EsAudio",
                table: "contenido$versionpartes");

            migrationBuilder.DropColumn(
                name: "EsPDF",
                table: "contenido$versionpartes");

            migrationBuilder.DropColumn(
                name: "EsVideo",
                table: "contenido$versionpartes");

            migrationBuilder.DropColumn(
                name: "Extension",
                table: "contenido$versionpartes");

            migrationBuilder.AlterColumn<string>(
                name: "TipoMime",
                table: "contenido$versionpartes",
                type: "varchar(50) CHARACTER SET utf8mb4",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 50,
                oldNullable: true);
        }
    }
}
