using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Seguridad.data.Migrations
{
    public partial class CambioUsuarioDominioUOFks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_seguridad$usuariosdominio_aspnetusers_ApplicationUserId",
                table: "seguridad$usuariosdominio",
                column: "ApplicationUserId",
                principalTable: "aspnetusers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_seguridad$usuariosdominio_aspnetusers_ApplicationUserId",
                table: "seguridad$usuariosdominio");
        }
    }
}
