using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.GestionDocumental.data.migrations
{
    public partial class permisosarchivo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "gd$permisosrchivo",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    ArchivoId = table.Column<string>(maxLength: 128, nullable: false),
                    DestinatarioId = table.Column<string>(maxLength: 128, nullable: false),
                    LeerAcervo = table.Column<bool>(nullable: false),
                    CrearAcervo = table.Column<bool>(nullable: false),
                    ActualizarAcervo = table.Column<bool>(nullable: false),
                    ElminarAcervo = table.Column<bool>(nullable: false),
                    CrearTrasnferencia = table.Column<bool>(nullable: false),
                    EliminarTrasnferencia = table.Column<bool>(nullable: false),
                    EnviarTrasnferencia = table.Column<bool>(nullable: false),
                    CancelarTrasnferencia = table.Column<bool>(nullable: false),
                    RecibirTrasnferencia = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$permisosrchivo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_gd$permisosrchivo_gd$archivo_ArchivoId",
                        column: x => x.ArchivoId,
                        principalTable: "gd$archivo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_gd$permisosrchivo_ArchivoId",
                table: "gd$permisosrchivo",
                column: "ArchivoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "gd$permisosrchivo");
        }
    }
}
