using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.GestionDocumental.Data.Migrations
{
    public partial class UnidadesAdminArchivos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "gd$unidadadministrativaarchivo",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    UnidadAdministrativa = table.Column<string>(maxLength: 200, nullable: false),
                    AreaProcedenciaArchivo = table.Column<string>(maxLength: 200, nullable: true),
                    Responsable = table.Column<string>(maxLength: 200, nullable: true),
                    Cargo = table.Column<string>(maxLength: 200, nullable: true),
                    Telefono = table.Column<string>(maxLength: 200, nullable: true),
                    Email = table.Column<string>(maxLength: 200, nullable: true),
                    Domicilio = table.Column<string>(maxLength: 200, nullable: true),
                    UbicacionFisica = table.Column<string>(maxLength: 500, nullable: true),
                    ArchivoId = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$unidadadministrativaarchivo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_gd$unidadadministrativaarchivo_gd$archivo_ArchivoId",
                        column: x => x.ArchivoId,
                        principalTable: "gd$archivo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_gd$unidadadministrativaarchivo_ArchivoId",
                table: "gd$unidadadministrativaarchivo",
                column: "ArchivoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "gd$unidadadministrativaarchivo");
        }
    }
}
