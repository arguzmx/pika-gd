using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.GestionDocumental.Data.migrations
{
    public partial class CorrecionActivoContenido : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddColumn<string>(
            //    name: "PuntoMontajeId",
            //    table: "gd$archivo",
            //    maxLength: 128,
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "VolumenDefaultId",
            //    table: "gd$archivo",
            //    maxLength: 128,
            //    nullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "TieneContenido",
                table: "gd$activo",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)",
                oldDefaultValue: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropColumn(
            //    name: "PuntoMontajeId",
            //    table: "gd$archivo");

            //migrationBuilder.DropColumn(
            //    name: "VolumenDefaultId",
            //    table: "gd$archivo");

            migrationBuilder.AlterColumn<bool>(
                name: "TieneContenido",
                table: "gd$activo",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldDefaultValue: false);
        }
    }
}
