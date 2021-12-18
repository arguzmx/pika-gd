using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.GestionDocumental.Data.Migrations
{
    public partial class PermisosUAdmin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UnidadAdministrativaArchivoId",
                table: "gd$activo",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "gd$permunidadadministrativa",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    UnidadAdministrativaArchivoId = table.Column<string>(maxLength: 128, nullable: false),
                    DestinatarioId = table.Column<string>(maxLength: 128, nullable: false),
                    LeerAcervo = table.Column<bool>(nullable: false),
                    CrearAcervo = table.Column<bool>(nullable: false),
                    ActualizarAcervo = table.Column<bool>(nullable: false),
                    ElminarAcervo = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$permunidadadministrativa", x => x.Id);
                    table.ForeignKey(
                        name: "FK_gd$permunidadadministrativa_gd$unidadadministrativaarchivo_U~",
                        column: x => x.UnidadAdministrativaArchivoId,
                        principalTable: "gd$unidadadministrativaarchivo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_gd$activo_UnidadAdministrativaArchivoId",
                table: "gd$activo",
                column: "UnidadAdministrativaArchivoId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$permunidadadministrativa_UnidadAdministrativaArchivoId",
                table: "gd$permunidadadministrativa",
                column: "UnidadAdministrativaArchivoId");

            migrationBuilder.AddForeignKey(
                name: "FK_gd$activo_gd$unidadadministrativaarchivo_UnidadAdministrativ~",
                table: "gd$activo",
                column: "UnidadAdministrativaArchivoId",
                principalTable: "gd$unidadadministrativaarchivo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_gd$activo_gd$unidadadministrativaarchivo_UnidadAdministrativ~",
                table: "gd$activo");

            migrationBuilder.DropTable(
                name: "gd$permunidadadministrativa");

            migrationBuilder.DropIndex(
                name: "IX_gd$activo_UnidadAdministrativaArchivoId",
                table: "gd$activo");

            migrationBuilder.DropColumn(
                name: "UnidadAdministrativaArchivoId",
                table: "gd$activo");
        }
    }
}
