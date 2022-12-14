using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.GestionDocumental.data.migrations
{
    public partial class RemoverAsuntos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "gd$asunto");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "gd$asunto",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false),
                    ActivoId = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", nullable: false),
                    Contenido = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$asunto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_gd$asunto_gd$activo_ActivoId",
                        column: x => x.ActivoId,
                        principalTable: "gd$activo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_gd$asunto_ActivoId",
                table: "gd$asunto",
                column: "ActivoId",
                unique: true);
        }
    }
}
