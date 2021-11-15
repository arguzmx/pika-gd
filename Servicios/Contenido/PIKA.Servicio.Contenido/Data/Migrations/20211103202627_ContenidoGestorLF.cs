using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Contenido.Data.Migrations
{
    public partial class ContenidoGestorLF : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "contenido$gestorlf",
                columns: table => new
                {
                    VolumenId = table.Column<string>(maxLength: 128, nullable: false),
                    Ruta = table.Column<string>(maxLength: 500, nullable: false),
                    VolumenId1 = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contenido$gestorlf", x => x.VolumenId);
                    table.ForeignKey(
                        name: "FK_contenido$gestorlf_contenido$volumen_VolumenId1",
                        column: x => x.VolumenId1,
                        principalTable: "contenido$volumen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_contenido$gestorlf_VolumenId1",
                table: "contenido$gestorlf",
                column: "VolumenId1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "contenido$gestorlf");
        }
    }
}
