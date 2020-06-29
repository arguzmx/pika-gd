using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Organizacion.Data.Migrations
{
    public partial class InicialOrganizacion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "org$dominio",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false),
                    Eliminada = table.Column<bool>(nullable: false, defaultValue: false),
                    TipoOrigenId = table.Column<string>(maxLength: 128, nullable: false),
                    OrigenId = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_org$dominio", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "org$rol",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(maxLength: 500, nullable: false),
                    TipoOrigenId = table.Column<string>(maxLength: 128, nullable: false),
                    OrigenId = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_org$rol", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "org$ou",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false),
                    Eliminada = table.Column<bool>(nullable: false, defaultValue: false),
                    DominioId = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_org$ou", x => x.Id);
                    table.ForeignKey(
                        name: "FK_org$ou_org$dominio_DominioId",
                        column: x => x.DominioId,
                        principalTable: "org$dominio",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "org$usuarios_rol",
                columns: table => new
                {
                    RolId = table.Column<string>(nullable: false),
                    ApplicationUserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_org$usuarios_rol", x => new { x.ApplicationUserId, x.RolId });
                    table.ForeignKey(
                        name: "FK_org$usuarios_rol_org$rol_RolId",
                        column: x => x.RolId,
                        principalTable: "org$rol",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_org$dominio_TipoOrigenId_OrigenId",
                table: "org$dominio",
                columns: new[] { "TipoOrigenId", "OrigenId" });

            migrationBuilder.CreateIndex(
                name: "IX_org$ou_DominioId",
                table: "org$ou",
                column: "DominioId");

            migrationBuilder.CreateIndex(
                name: "IX_org$rol_TipoOrigenId_OrigenId",
                table: "org$rol",
                columns: new[] { "TipoOrigenId", "OrigenId" });

            migrationBuilder.CreateIndex(
                name: "IX_org$usuarios_rol_RolId",
                table: "org$usuarios_rol",
                column: "RolId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "org$ou");

            migrationBuilder.DropTable(
                name: "org$usuarios_rol");

            migrationBuilder.DropTable(
                name: "org$dominio");

            migrationBuilder.DropTable(
                name: "org$rol");
        }
    }
}
