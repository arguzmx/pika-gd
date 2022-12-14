using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.GestionDocumental.data.migrations
{
    public partial class UpdateSeguridad : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Tipo",
                table: "gd$tipoarchivo",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "gd$activo",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioId",
                table: "gd$activo",
                maxLength: 128,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_gd$entradaclasificacion_CuadroClasifiacionId",
                table: "gd$entradaclasificacion",
                column: "CuadroClasifiacionId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$activo_FechaCreacion",
                table: "gd$activo",
                column: "FechaCreacion");

            migrationBuilder.CreateIndex(
                name: "IX_gd$activo_UsuarioId",
                table: "gd$activo",
                column: "UsuarioId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_gd$entradaclasificacion_CuadroClasifiacionId",
                table: "gd$entradaclasificacion");

            migrationBuilder.DropIndex(
                name: "IX_gd$activo_FechaCreacion",
                table: "gd$activo");

            migrationBuilder.DropIndex(
                name: "IX_gd$activo_UsuarioId",
                table: "gd$activo");

            migrationBuilder.DropColumn(
                name: "Tipo",
                table: "gd$tipoarchivo");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "gd$activo");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "gd$activo");
        }
    }
}
