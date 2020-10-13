using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Seguridad.data.Migrations
{
    public partial class CambioUsuarioDominioUO : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_seguridad$usuariosdominio_aspnetusers_ApplicationUserId",
                table: "seguridad$usuariosdominio");

            migrationBuilder.DropPrimaryKey(
                name: "PK_seguridad$usuariosdominio",
                table: "seguridad$usuariosdominio");

            migrationBuilder.DropColumn(
                name: "TipoOrigenId",
                table: "seguridad$usuariosdominio");

            migrationBuilder.DropColumn(
                name: "OrigenId",
                table: "seguridad$usuariosdominio");

            migrationBuilder.AddColumn<string>(
                name: "DominioId",
                table: "seguridad$usuariosdominio",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UnidadOrganizacionalId",
                table: "seguridad$usuariosdominio",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_seguridad$usuariosdominio",
                table: "seguridad$usuariosdominio",
                columns: new[] { "ApplicationUserId", "DominioId", "UnidadOrganizacionalId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_seguridad$usuariosdominio",
                table: "seguridad$usuariosdominio");

            migrationBuilder.DropColumn(
                name: "DominioId",
                table: "seguridad$usuariosdominio");

            migrationBuilder.DropColumn(
                name: "UnidadOrganizacionalId",
                table: "seguridad$usuariosdominio");

            migrationBuilder.AddColumn<string>(
                name: "TipoOrigenId",
                table: "seguridad$usuariosdominio",
                type: "varchar(128) CHARACTER SET utf8mb4",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OrigenId",
                table: "seguridad$usuariosdominio",
                type: "varchar(128) CHARACTER SET utf8mb4",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_seguridad$usuariosdominio",
                table: "seguridad$usuariosdominio",
                columns: new[] { "ApplicationUserId", "TipoOrigenId", "OrigenId" });

            migrationBuilder.AddForeignKey(
                name: "FK_seguridad$usuariosdominio_aspnetusers_ApplicationUserId",
                table: "seguridad$usuariosdominio",
                column: "ApplicationUserId",
                principalTable: "aspnetusers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
