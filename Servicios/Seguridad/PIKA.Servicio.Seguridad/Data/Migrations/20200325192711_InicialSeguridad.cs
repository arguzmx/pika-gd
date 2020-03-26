using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Seguridad.Data.Migrations
{
    public partial class InicialSeguridad : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "seguridad$aplicacion",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(maxLength: 500, nullable: false),
                    UICulture = table.Column<string>(maxLength: 10, nullable: false),
                    Version = table.Column<string>(maxLength: 10, nullable: false),
                    ReleaseIndex = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_seguridad$aplicacion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "seguridad$moduloaplicacion",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    AplicacionId = table.Column<string>(maxLength: 128, nullable: false),
                    Asegurable = table.Column<bool>(nullable: false, defaultValue: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(maxLength: 500, nullable: false),
                    ModuloPadreId = table.Column<string>(nullable: true),
                    AplicacionPadreId = table.Column<string>(nullable: true),
                    Icono = table.Column<string>(maxLength: 100, nullable: false),
                    UICulture = table.Column<string>(maxLength: 10, nullable: false),
                    PermisosDisponibles = table.Column<ulong>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_seguridad$moduloaplicacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_seguridad$moduloaplicacion_seguridad$aplicacion_AplicacionId",
                        column: x => x.AplicacionId,
                        principalTable: "seguridad$aplicacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_seguridad$moduloaplicacion_seguridad$moduloaplicacion_Modulo~",
                        column: x => x.ModuloPadreId,
                        principalTable: "seguridad$moduloaplicacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "seguridad$tipoadministradormodulo",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    AplicacionId = table.Column<string>(maxLength: 128, nullable: false),
                    ModuloId = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_seguridad$tipoadministradormodulo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_seguridad$tipoadministradormodulo_seguridad$moduloaplicacion~",
                        column: x => x.ModuloId,
                        principalTable: "seguridad$moduloaplicacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "seguridad$traduccionaplicacionmodulo",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    AplicacionId = table.Column<string>(maxLength: 128, nullable: false),
                    ModuloId = table.Column<string>(maxLength: 128, nullable: false),
                    UICulture = table.Column<string>(maxLength: 10, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_seguridad$traduccionaplicacionmodulo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_seguridad$traduccionaplicacionmodulo_seguridad$aplicacion_Ap~",
                        column: x => x.AplicacionId,
                        principalTable: "seguridad$aplicacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_seguridad$traduccionaplicacionmodulo_seguridad$moduloaplicac~",
                        column: x => x.ModuloId,
                        principalTable: "seguridad$moduloaplicacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_seguridad$moduloaplicacion_AplicacionId",
                table: "seguridad$moduloaplicacion",
                column: "AplicacionId");

            migrationBuilder.CreateIndex(
                name: "IX_seguridad$moduloaplicacion_ModuloPadreId",
                table: "seguridad$moduloaplicacion",
                column: "ModuloPadreId");

            migrationBuilder.CreateIndex(
                name: "IX_seguridad$tipoadministradormodulo_ModuloId",
                table: "seguridad$tipoadministradormodulo",
                column: "ModuloId");

            migrationBuilder.CreateIndex(
                name: "IX_seguridad$traduccionaplicacionmodulo_AplicacionId",
                table: "seguridad$traduccionaplicacionmodulo",
                column: "AplicacionId");

            migrationBuilder.CreateIndex(
                name: "IX_seguridad$traduccionaplicacionmodulo_ModuloId",
                table: "seguridad$traduccionaplicacionmodulo",
                column: "ModuloId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "seguridad$tipoadministradormodulo");

            migrationBuilder.DropTable(
                name: "seguridad$traduccionaplicacionmodulo");

            migrationBuilder.DropTable(
                name: "seguridad$moduloaplicacion");

            migrationBuilder.DropTable(
                name: "seguridad$aplicacion");
        }
    }
}
