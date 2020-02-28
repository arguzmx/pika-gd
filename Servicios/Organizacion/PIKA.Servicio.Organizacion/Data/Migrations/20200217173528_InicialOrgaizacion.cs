using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Organizacion.Data.Migrations
{
    public partial class InicialOrgaizacion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "org$dominio",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_org$dominio", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "org$ou",
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
                    table.PrimaryKey("PK_org$ou", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "org$pais",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Valor = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_org$pais", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "org$rol",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(maxLength: 500, nullable: true),
                    TipoOrigenId = table.Column<string>(maxLength: 128, nullable: false),
                    OrigenId = table.Column<string>(maxLength: 128, nullable: false),
                    RolPadreId = table.Column<string>(maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_org$rol", x => x.Id);
                    table.ForeignKey(
                        name: "FK_org$rol_org$rol_RolPadreId",
                        column: x => x.RolPadreId,
                        principalTable: "org$rol",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "org$estado",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Valor = table.Column<string>(maxLength: 200, nullable: false),
                    PaisId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_org$estado", x => x.Id);
                    table.ForeignKey(
                        name: "FK_org$estado_org$pais_PaisId",
                        column: x => x.PaisId,
                        principalTable: "org$pais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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

            migrationBuilder.CreateTable(
                name: "org$direccion_postal",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: true),
                    Calle = table.Column<string>(maxLength: 200, nullable: true),
                    NoInterno = table.Column<string>(maxLength: 200, nullable: true),
                    NoExterno = table.Column<string>(maxLength: 200, nullable: true),
                    Colonia = table.Column<string>(maxLength: 200, nullable: true),
                    CP = table.Column<string>(maxLength: 10, nullable: true),
                    Municipio = table.Column<string>(maxLength: 200, nullable: true),
                    EstadoId = table.Column<string>(maxLength: 128, nullable: true),
                    PaisId = table.Column<string>(maxLength: 128, nullable: true),
                    TipoOrigenId = table.Column<string>(maxLength: 128, nullable: false),
                    OrigenId = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_org$direccion_postal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_org$direccion_postal_org$estado_EstadoId",
                        column: x => x.EstadoId,
                        principalTable: "org$estado",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_org$direccion_postal_org$pais_PaisId",
                        column: x => x.PaisId,
                        principalTable: "org$pais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_org$direccion_postal_EstadoId",
                table: "org$direccion_postal",
                column: "EstadoId");

            migrationBuilder.CreateIndex(
                name: "IX_org$direccion_postal_PaisId",
                table: "org$direccion_postal",
                column: "PaisId");

            migrationBuilder.CreateIndex(
                name: "IX_org$direccion_postal_TipoOrigenId_OrigenId",
                table: "org$direccion_postal",
                columns: new[] { "TipoOrigenId", "OrigenId" });

            migrationBuilder.CreateIndex(
                name: "IX_org$estado_PaisId",
                table: "org$estado",
                column: "PaisId");

            migrationBuilder.CreateIndex(
                name: "IX_org$ou_TipoOrigenId_OrigenId",
                table: "org$ou",
                columns: new[] { "TipoOrigenId", "OrigenId" });

            migrationBuilder.CreateIndex(
                name: "IX_org$rol_RolPadreId",
                table: "org$rol",
                column: "RolPadreId");

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
                name: "org$direccion_postal");

            migrationBuilder.DropTable(
                name: "org$dominio");

            migrationBuilder.DropTable(
                name: "org$ou");

            migrationBuilder.DropTable(
                name: "org$usuarios_rol");

            migrationBuilder.DropTable(
                name: "org$estado");

            migrationBuilder.DropTable(
                name: "org$rol");

            migrationBuilder.DropTable(
                name: "org$pais");
        }
    }
}
