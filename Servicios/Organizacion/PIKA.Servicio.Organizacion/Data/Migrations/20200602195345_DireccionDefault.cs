using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Organizacion.Data.migrations
{
    public partial class DireccionDefault : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Default",
                table: "org$direccion_postal",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Default",
                table: "org$direccion_postal");
        }
    }
}
