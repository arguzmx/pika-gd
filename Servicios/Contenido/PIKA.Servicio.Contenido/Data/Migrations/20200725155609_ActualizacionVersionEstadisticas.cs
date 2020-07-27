using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Contenido.data.Migrations
{
    public partial class ActualizacionVersionEstadisticas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "Eliminada",
                table: "contenido$versionelemento",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "Activa",
                table: "contenido$versionelemento",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)",
                oldDefaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ConteoPartes",
                table: "contenido$versionelemento",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxIndicePartes",
                table: "contenido$versionelemento",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "TamanoBytes",
                table: "contenido$versionelemento",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConteoPartes",
                table: "contenido$versionelemento");

            migrationBuilder.DropColumn(
                name: "MaxIndicePartes",
                table: "contenido$versionelemento");

            migrationBuilder.DropColumn(
                name: "TamanoBytes",
                table: "contenido$versionelemento");

            migrationBuilder.AlterColumn<bool>(
                name: "Eliminada",
                table: "contenido$versionelemento",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<bool>(
                name: "Activa",
                table: "contenido$versionelemento",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool));
        }
    }
}
