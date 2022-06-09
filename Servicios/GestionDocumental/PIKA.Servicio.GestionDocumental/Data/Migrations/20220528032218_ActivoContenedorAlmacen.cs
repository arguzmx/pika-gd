using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.GestionDocumental.Data.Migrations
{
    public partial class ActivoContenedorAlmacen : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "gd$activocontalmacen",
                columns: table => new
                {
                    ContenedorAlmacenId = table.Column<string>(nullable: false),
                    ActivoId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$activocontalmacen", x => new { x.ContenedorAlmacenId, x.ActivoId });
                    table.ForeignKey(
                        name: "FK_gd$activocontalmacen_gd$contalmacen_ContenedorAlmacenId",
                        column: x => x.ContenedorAlmacenId,
                        principalTable: "gd$contalmacen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "gd$activocontalmacen");
        }
    }
}
