using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Seguridad.Data.Migrations
{
    public partial class ValoresClaveUsuario : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "seguridad$valorclave",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    UsuarioId = table.Column<string>(maxLength: 255, nullable: false),
                    Clave = table.Column<string>(maxLength: 200, nullable: false),
                    Valor = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_seguridad$valorclave", x => x.Id);
                    table.ForeignKey(
                        name: "FK_seguridad$valorclave_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_seguridad$valorclave_UsuarioId",
                table: "seguridad$valorclave",
                column: "UsuarioId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "seguridad$valorclave");
        }
    }
}
