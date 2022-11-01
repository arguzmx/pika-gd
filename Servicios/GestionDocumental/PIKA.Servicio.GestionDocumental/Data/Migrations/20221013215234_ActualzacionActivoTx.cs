using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.GestionDocumental.data.migrations
{
    public partial class ActualzacionActivoTx : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_gd$activotransferencia_UsuarioIdDeclinadoId",
                table: "gd$activotransferencia");

            migrationBuilder.DropColumn(
                name: "UsuarioIdDeclinadoId",
                table: "gd$activotransferencia");

            migrationBuilder.AddColumn<bool>(
                name: "Aceptado",
                table: "gd$activotransferencia",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaVoto",
                table: "gd$activotransferencia",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notas",
                table: "gd$activotransferencia",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioId",
                table: "gd$activotransferencia",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioReceptorId",
                table: "gd$activotransferencia",
                maxLength: 128,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_gd$activotransferencia_UsuarioId",
                table: "gd$activotransferencia",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$activotransferencia_UsuarioReceptorId",
                table: "gd$activotransferencia",
                column: "UsuarioReceptorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_gd$activotransferencia_UsuarioId",
                table: "gd$activotransferencia");

            migrationBuilder.DropIndex(
                name: "IX_gd$activotransferencia_UsuarioReceptorId",
                table: "gd$activotransferencia");

            migrationBuilder.DropColumn(
                name: "Aceptado",
                table: "gd$activotransferencia");

            migrationBuilder.DropColumn(
                name: "FechaVoto",
                table: "gd$activotransferencia");

            migrationBuilder.DropColumn(
                name: "Notas",
                table: "gd$activotransferencia");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "gd$activotransferencia");

            migrationBuilder.DropColumn(
                name: "UsuarioReceptorId",
                table: "gd$activotransferencia");

            migrationBuilder.AddColumn<string>(
                name: "UsuarioIdDeclinadoId",
                table: "gd$activotransferencia",
                type: "varchar(128) CHARACTER SET utf8mb4",
                maxLength: 128,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_gd$activotransferencia_UsuarioIdDeclinadoId",
                table: "gd$activotransferencia",
                column: "UsuarioIdDeclinadoId");
        }
    }
}
