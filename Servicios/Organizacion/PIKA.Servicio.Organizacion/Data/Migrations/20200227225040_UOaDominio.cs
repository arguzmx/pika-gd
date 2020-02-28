using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Organizacion.Data.Migrations
{
    public partial class UOaDominio : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_org$ou_TipoOrigenId_OrigenId",
                table: "org$ou");

            migrationBuilder.DropColumn(
                name: "OrigenId",
                table: "org$ou");

            migrationBuilder.DropColumn(
                name: "TipoOrigenId",
                table: "org$ou");

            migrationBuilder.AddColumn<string>(
                name: "DominioId",
                table: "org$ou",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrigenId",
                table: "org$dominio",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TipoOrigenId",
                table: "org$dominio",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_org$ou_DominioId",
                table: "org$ou",
                column: "DominioId");

            migrationBuilder.CreateIndex(
                name: "IX_org$dominio_TipoOrigenId_OrigenId",
                table: "org$dominio",
                columns: new[] { "TipoOrigenId", "OrigenId" });

            migrationBuilder.AddForeignKey(
                name: "FK_org$ou_org$dominio_DominioId",
                table: "org$ou",
                column: "DominioId",
                principalTable: "org$dominio",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_org$ou_org$dominio_DominioId",
                table: "org$ou");

            migrationBuilder.DropIndex(
                name: "IX_org$ou_DominioId",
                table: "org$ou");

            migrationBuilder.DropIndex(
                name: "IX_org$dominio_TipoOrigenId_OrigenId",
                table: "org$dominio");

            migrationBuilder.DropColumn(
                name: "DominioId",
                table: "org$ou");

            migrationBuilder.DropColumn(
                name: "OrigenId",
                table: "org$dominio");

            migrationBuilder.DropColumn(
                name: "TipoOrigenId",
                table: "org$dominio");

            migrationBuilder.AddColumn<string>(
                name: "OrigenId",
                table: "org$ou",
                type: "varchar(128) CHARACTER SET utf8mb4",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TipoOrigenId",
                table: "org$ou",
                type: "varchar(128) CHARACTER SET utf8mb4",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_org$ou_TipoOrigenId_OrigenId",
                table: "org$ou",
                columns: new[] { "TipoOrigenId", "OrigenId" });
        }
    }
}
