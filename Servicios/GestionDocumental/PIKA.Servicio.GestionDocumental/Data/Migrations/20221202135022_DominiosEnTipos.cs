using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.GestionDocumental.Data.Migrations
{
    public partial class DominiosEnTipos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DominioId",
                table: "gd$tipovaloraciondocumental",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UOId",
                table: "gd$tipovaloraciondocumental",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DominioId",
                table: "gd$tipodisposiciondocumental",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UOId",
                table: "gd$tipodisposiciondocumental",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DominioId",
                table: "gd$tipoarchivo",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UOId",
                table: "gd$tipoarchivo",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DominioId",
                table: "gd$tipoampliacion",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UOId",
                table: "gd$tipoampliacion",
                maxLength: 128,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_gd$tipovaloraciondocumental_DominioId",
                table: "gd$tipovaloraciondocumental",
                column: "DominioId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$tipodisposiciondocumental_DominioId",
                table: "gd$tipodisposiciondocumental",
                column: "DominioId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$tipoarchivo_DominioId",
                table: "gd$tipoarchivo",
                column: "DominioId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$tipoampliacion_DominioId",
                table: "gd$tipoampliacion",
                column: "DominioId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_gd$tipovaloraciondocumental_DominioId",
                table: "gd$tipovaloraciondocumental");

            migrationBuilder.DropIndex(
                name: "IX_gd$tipodisposiciondocumental_DominioId",
                table: "gd$tipodisposiciondocumental");

            migrationBuilder.DropIndex(
                name: "IX_gd$tipoarchivo_DominioId",
                table: "gd$tipoarchivo");

            migrationBuilder.DropIndex(
                name: "IX_gd$tipoampliacion_DominioId",
                table: "gd$tipoampliacion");

            migrationBuilder.DropColumn(
                name: "DominioId",
                table: "gd$tipovaloraciondocumental");

            migrationBuilder.DropColumn(
                name: "UOId",
                table: "gd$tipovaloraciondocumental");

            migrationBuilder.DropColumn(
                name: "DominioId",
                table: "gd$tipodisposiciondocumental");

            migrationBuilder.DropColumn(
                name: "UOId",
                table: "gd$tipodisposiciondocumental");

            migrationBuilder.DropColumn(
                name: "DominioId",
                table: "gd$tipoarchivo");

            migrationBuilder.DropColumn(
                name: "UOId",
                table: "gd$tipoarchivo");

            migrationBuilder.DropColumn(
                name: "DominioId",
                table: "gd$tipoampliacion");

            migrationBuilder.DropColumn(
                name: "UOId",
                table: "gd$tipoampliacion");
        }
    }
}
