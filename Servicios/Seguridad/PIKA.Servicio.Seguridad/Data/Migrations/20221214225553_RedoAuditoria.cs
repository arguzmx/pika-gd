using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Seguridad.data.migrations
{
    public partial class RedoAuditoria : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "seguridad$eventosaud",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    DireccionRed = table.Column<string>(maxLength: 128, nullable: false),
                    IdSesion = table.Column<string>(maxLength: 128, nullable: true),
                    UsuarioId = table.Column<string>(maxLength: 128, nullable: false),
                    DominioId = table.Column<string>(maxLength: 128, nullable: false),
                    UAId = table.Column<string>(maxLength: 128, nullable: false),
                    Exitoso = table.Column<bool>(nullable: false),
                    Fuente = table.Column<string>(maxLength: 128, nullable: false),
                    AppId = table.Column<string>(maxLength: 128, nullable: false),
                    ModuloId = table.Column<string>(maxLength: 128, nullable: false),
                    TipoEvento = table.Column<int>(nullable: false),
                    TipoFalla = table.Column<int>(nullable: true),
                    TipoEntidad = table.Column<string>(maxLength: 128, nullable: true),
                    IdEntidad = table.Column<string>(maxLength: 128, nullable: true),
                    NombreEntidad = table.Column<string>(maxLength: 500, nullable: true),
                    Delta = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_seguridad$eventosaud", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "seguridad$eventosaudconf",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    DominioId = table.Column<string>(nullable: false),
                    UAId = table.Column<string>(maxLength: 128, nullable: false),
                    AppId = table.Column<string>(maxLength: 128, nullable: false),
                    ModuloId = table.Column<string>(maxLength: 128, nullable: false),
                    TipoEvento = table.Column<int>(nullable: false),
                    Auditar = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_seguridad$eventosaudconf", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_seguridad$eventosaud_all",
                table: "seguridad$eventosaud",
                columns: new[] { "DominioId", "UAId", "AppId", "ModuloId", "UsuarioId" });

            migrationBuilder.CreateIndex(
                name: "IX_seguridad$eventosaudconf_all",
                table: "seguridad$eventosaudconf",
                columns: new[] { "DominioId", "UAId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "seguridad$eventosaud");

            migrationBuilder.DropTable(
                name: "seguridad$eventosaudconf");
        }
    }
}
