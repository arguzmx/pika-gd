using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Contenido.data.Migrations
{
    public partial class ConfiguracionGestoresES : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CadenaConexion",
                table: "contenido$volumen");

            migrationBuilder.CreateTable(
                name: "contenido$gestorazure",
                columns: table => new
                {
                    VolumenId = table.Column<string>(maxLength: 128, nullable: false),
                    Endpoint = table.Column<string>(maxLength: 500, nullable: false),
                    Usuario = table.Column<string>(maxLength: 200, nullable: false),
                    Contraena = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contenido$gestorazure", x => x.VolumenId);
                    table.ForeignKey(
                        name: "FK_contenido$gestorazure_contenido$volumen_VolumenId",
                        column: x => x.VolumenId,
                        principalTable: "contenido$volumen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "contenido$gestorlocal",
                columns: table => new
                {
                    VolumenId = table.Column<string>(maxLength: 128, nullable: false),
                    Ruta = table.Column<string>(maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contenido$gestorlocal", x => x.VolumenId);
                    table.ForeignKey(
                        name: "FK_contenido$gestorlocal_contenido$volumen_VolumenId",
                        column: x => x.VolumenId,
                        principalTable: "contenido$volumen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "contenido$gestorsmb",
                columns: table => new
                {
                    VolumenId = table.Column<string>(maxLength: 128, nullable: false),
                    Ruta = table.Column<string>(maxLength: 500, nullable: false),
                    Dominio = table.Column<string>(maxLength: 200, nullable: false),
                    Usuario = table.Column<string>(maxLength: 200, nullable: false),
                    Contraena = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contenido$gestorsmb", x => x.VolumenId);
                    table.ForeignKey(
                        name: "FK_contenido$gestorsmb_contenido$volumen_VolumenId",
                        column: x => x.VolumenId,
                        principalTable: "contenido$volumen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "contenido$gestorazure");

            migrationBuilder.DropTable(
                name: "contenido$gestorlocal");

            migrationBuilder.DropTable(
                name: "contenido$gestorsmb");

            migrationBuilder.AddColumn<string>(
                name: "CadenaConexion",
                table: "contenido$volumen",
                type: "varchar(2000) CHARACTER SET utf8mb4",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");
        }
    }
}
