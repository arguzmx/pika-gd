using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Reportes.Migrations
{
    public partial class CreacionEntidadReportes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "repo$reporteentidad",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Entidad = table.Column<string>(maxLength: 500, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(maxLength: 500, nullable: false),
                    Plantilla = table.Column<string>(maxLength: 512, nullable: false),
                    TipoOrigenId = table.Column<string>(maxLength: 128, nullable: false),
                    OrigenId = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_repo$reporteentidad", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_repo$reporteentidad_Entidad",
                table: "repo$reporteentidad",
                column: "Entidad");

            migrationBuilder.CreateIndex(
                name: "IX_repo$reporteentidad_OrigenId",
                table: "repo$reporteentidad",
                column: "OrigenId");

            migrationBuilder.CreateIndex(
                name: "IX_repo$reporteentidad_TipoOrigenId",
                table: "repo$reporteentidad",
                column: "TipoOrigenId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "repo$reporteentidad");
        }
    }
}
