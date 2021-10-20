using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Contenido.Data.Migrations
{
    public partial class PermisosPuntoMontaje : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "contenido$permpmontaje",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    PuntoMontajeIdId = table.Column<string>(maxLength: 128, nullable: false),
                    DestinatarioId = table.Column<string>(maxLength: 128, nullable: false),
                    CrearCarpeta = table.Column<bool>(nullable: false),
                    ActualizarCarpeta = table.Column<bool>(nullable: false),
                    ElminarCarpeta = table.Column<bool>(nullable: false),
                    CrearDocumento = table.Column<bool>(nullable: false),
                    ActualizarDocumento = table.Column<bool>(nullable: false),
                    ElminarDocumento = table.Column<bool>(nullable: false),
                    GestionContenido = table.Column<bool>(nullable: false),
                    GestionMetadatos = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contenido$permpmontaje", x => x.Id);
                    table.ForeignKey(
                        name: "FK_contenido$permpmontaje_contenido$pmontaje_PuntoMontajeIdId",
                        column: x => x.PuntoMontajeIdId,
                        principalTable: "contenido$pmontaje",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_contenido$permpmontaje_PuntoMontajeIdId",
                table: "contenido$permpmontaje",
                column: "PuntoMontajeIdId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "contenido$permpmontaje");
        }
    }
}
