using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.GestionDocumental.data.migrations
{
    public partial class ActivoEnTransferencia : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "Ampliado",
                table: "gd$activo",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)");

            migrationBuilder.AddColumn<bool>(
                name: "EnTransferencia",
                table: "gd$activo",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_gd$activo_EnTransferencia",
                table: "gd$activo",
                column: "EnTransferencia");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_gd$activo_EnTransferencia",
                table: "gd$activo");

            migrationBuilder.DropColumn(
                name: "EnTransferencia",
                table: "gd$activo");

            migrationBuilder.AlterColumn<bool>(
                name: "Ampliado",
                table: "gd$activo",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldDefaultValue: false);
        }
    }
}
