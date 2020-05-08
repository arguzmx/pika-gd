using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.GestionDocumental.Data.Migrations
{
    public partial class CuadroClasificacion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "gd$estadocuadroclasificacion",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$estadocuadroclasificacion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "gd$cuadroclasificacion",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false),
                    Eliminada = table.Column<bool>(nullable: false, defaultValue: false),
                    TipoOrigenId = table.Column<string>(maxLength: 128, nullable: false),
                    OrigenId = table.Column<string>(maxLength: 128, nullable: false),
                    EstadoCuadroClasificacionId = table.Column<string>(maxLength: 128, nullable: false, defaultValue: "on")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$cuadroclasificacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_gd$cuadroclasificacion_gd$estadocuadroclasificacion_EstadoCu~",
                        column: x => x.EstadoCuadroClasificacionId,
                        principalTable: "gd$estadocuadroclasificacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "gd$elementoclasificacion",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    ElementoClasificacionId = table.Column<string>(nullable: true),
                    Clave = table.Column<string>(maxLength: 200, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false),
                    Eliminada = table.Column<bool>(nullable: false, defaultValue: false),
                    Posicion = table.Column<int>(maxLength: 10, nullable: false),
                    CuadroClasifiacionId = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$elementoclasificacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_gd$elementoclasificacion_gd$cuadroclasificacion_CuadroClasif~",
                        column: x => x.CuadroClasifiacionId,
                        principalTable: "gd$cuadroclasificacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_gd$elementoclasificacion_gd$elementoclasificacion_ElementoCl~",
                        column: x => x.ElementoClasificacionId,
                        principalTable: "gd$elementoclasificacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_gd$cuadroclasificacion_EstadoCuadroClasificacionId",
                table: "gd$cuadroclasificacion",
                column: "EstadoCuadroClasificacionId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$elementoclasificacion_CuadroClasifiacionId",
                table: "gd$elementoclasificacion",
                column: "CuadroClasifiacionId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$elementoclasificacion_ElementoClasificacionId",
                table: "gd$elementoclasificacion",
                column: "ElementoClasificacionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "gd$elementoclasificacion");

            migrationBuilder.DropTable(
                name: "gd$cuadroclasificacion");

            migrationBuilder.DropTable(
                name: "gd$estadocuadroclasificacion");
        }
    }
}
