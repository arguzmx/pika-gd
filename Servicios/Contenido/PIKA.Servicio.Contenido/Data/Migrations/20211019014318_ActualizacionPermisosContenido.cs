using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Contenido.Data.Migrations
{
    public partial class ActualizacionPermisosContenido : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActualizarCarpeta",
                table: "contenido$permpmontaje");

            migrationBuilder.DropColumn(
                name: "ActualizarDocumento",
                table: "contenido$permpmontaje");

            migrationBuilder.DropColumn(
                name: "CrearCarpeta",
                table: "contenido$permpmontaje");

            migrationBuilder.DropColumn(
                name: "CrearDocumento",
                table: "contenido$permpmontaje");

            migrationBuilder.DropColumn(
                name: "ElminarCarpeta",
                table: "contenido$permpmontaje");

            migrationBuilder.DropColumn(
                name: "ElminarDocumento",
                table: "contenido$permpmontaje");

            migrationBuilder.DropColumn(
                name: "LeerContenido",
                table: "contenido$permpmontaje");

            migrationBuilder.AddColumn<bool>(
                name: "Actualizar",
                table: "contenido$permpmontaje",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Crear",
                table: "contenido$permpmontaje",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Elminar",
                table: "contenido$permpmontaje",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Leer",
                table: "contenido$permpmontaje",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Actualizar",
                table: "contenido$permpmontaje");

            migrationBuilder.DropColumn(
                name: "Crear",
                table: "contenido$permpmontaje");

            migrationBuilder.DropColumn(
                name: "Elminar",
                table: "contenido$permpmontaje");

            migrationBuilder.DropColumn(
                name: "Leer",
                table: "contenido$permpmontaje");

            migrationBuilder.AddColumn<bool>(
                name: "ActualizarCarpeta",
                table: "contenido$permpmontaje",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ActualizarDocumento",
                table: "contenido$permpmontaje",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CrearCarpeta",
                table: "contenido$permpmontaje",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CrearDocumento",
                table: "contenido$permpmontaje",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ElminarCarpeta",
                table: "contenido$permpmontaje",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ElminarDocumento",
                table: "contenido$permpmontaje",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "LeerContenido",
                table: "contenido$permpmontaje",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }
    }
}
