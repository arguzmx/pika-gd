using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.GestionDocumental.Data.migrations
{
    public partial class SeleccionActivos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "gd$temasactivos",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    UsuarioId = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$temasactivos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "gd$activoseleccionado",
                columns: table => new
                {
                    UsuarioId = table.Column<string>(maxLength: 128, nullable: false),
                    TemaId = table.Column<string>(maxLength: 128, nullable: false),
                    Id = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$activoseleccionado", x => new { x.Id, x.UsuarioId, x.TemaId });
                    table.ForeignKey(
                        name: "FK_gd$activoseleccionado_gd$activo_Id",
                        column: x => x.Id,
                        principalTable: "gd$activo",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_gd$activoseleccionado_gd$temasactivos_TemaId",
                        column: x => x.TemaId,
                        principalTable: "gd$temasactivos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_gd$activoseleccionado_TemaId",
                table: "gd$activoseleccionado",
                column: "TemaId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$temasactivos_UsuarioId",
                table: "gd$temasactivos",
                column: "UsuarioId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "gd$activoseleccionado");

            migrationBuilder.DropTable(
                name: "gd$temasactivos");
        }
    }
}
