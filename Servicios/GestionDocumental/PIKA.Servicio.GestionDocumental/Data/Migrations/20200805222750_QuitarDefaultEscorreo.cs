using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.GestionDocumental.Data.Migrations
{
    public partial class QuitarDefaultEscorreo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "Reservado",
                table: "gd$activo",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "EsElectronio",
                table: "gd$activo",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<bool>(
                name: "EnPrestamo",
                table: "gd$activo",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "Confidencial",
                table: "gd$activo",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "Ampliado",
                table: "gd$activo",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)",
                oldDefaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "Reservado",
                table: "gd$activo",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<bool>(
                name: "EsElectronio",
                table: "gd$activo",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<bool>(
                name: "EnPrestamo",
                table: "gd$activo",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<bool>(
                name: "Confidencial",
                table: "gd$activo",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<bool>(
                name: "Ampliado",
                table: "gd$activo",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool));
        }
    }
}
