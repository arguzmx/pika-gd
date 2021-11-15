using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Contenido.Data.Migrations
{
    public partial class ContenidoGestorLF2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_contenido$gestorlf_contenido$volumen_VolumenId1",
                table: "contenido$gestorlf");

            migrationBuilder.DropIndex(
                name: "IX_contenido$gestorlf_VolumenId1",
                table: "contenido$gestorlf");

            migrationBuilder.DropColumn(
                name: "VolumenId1",
                table: "contenido$gestorlf");

            migrationBuilder.AddForeignKey(
                name: "FK_contenido$gestorlf_contenido$volumen_VolumenId",
                table: "contenido$gestorlf",
                column: "VolumenId",
                principalTable: "contenido$volumen",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_contenido$gestorlf_contenido$volumen_VolumenId",
                table: "contenido$gestorlf");

            migrationBuilder.AddColumn<string>(
                name: "VolumenId1",
                table: "contenido$gestorlf",
                type: "varchar(128) CHARACTER SET utf8mb4",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_contenido$gestorlf_VolumenId1",
                table: "contenido$gestorlf",
                column: "VolumenId1");

            migrationBuilder.AddForeignKey(
                name: "FK_contenido$gestorlf_contenido$volumen_VolumenId1",
                table: "contenido$gestorlf",
                column: "VolumenId1",
                principalTable: "contenido$volumen",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
