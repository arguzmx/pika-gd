using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Contenido.Data.Migrations
{
    public partial class LongitudMIME : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TipoMime",
                table: "contenido$versionpartes",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50) CHARACTER SET utf8mb4",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Extension",
                table: "contenido$versionpartes",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50) CHARACTER SET utf8mb4",
                oldMaxLength: 50);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TipoMime",
                table: "contenido$versionpartes",
                type: "varchar(50) CHARACTER SET utf8mb4",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Extension",
                table: "contenido$versionpartes",
                type: "varchar(50) CHARACTER SET utf8mb4",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 100);
        }
    }
}
