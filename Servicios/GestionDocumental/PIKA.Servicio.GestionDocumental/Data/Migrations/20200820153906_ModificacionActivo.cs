using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.GestionDocumental.data.Migrations
{
    public partial class ModificacionActivo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CodigoOptico",
                table: "gd$activo",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1024) CHARACTER SET utf8mb4",
                oldMaxLength: 1024,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CodigoElectronico",
                table: "gd$activo",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1024) CHARACTER SET utf8mb4",
                oldMaxLength: 1024,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ArchivoId",
                table: "gd$activo",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(128) CHARACTER SET utf8mb4",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ArchivoOrigenId",
                table: "gd$activo",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaRetencionAC",
                table: "gd$activo",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaRetencionAT",
                table: "gd$activo",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_gd$activo_Ampliado",
                table: "gd$activo",
                column: "Ampliado");

            migrationBuilder.CreateIndex(
                name: "IX_gd$activo_ArchivoOrigenId",
                table: "gd$activo",
                column: "ArchivoOrigenId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$activo_CodigoElectronico",
                table: "gd$activo",
                column: "CodigoElectronico");

            migrationBuilder.CreateIndex(
                name: "IX_gd$activo_CodigoOptico",
                table: "gd$activo",
                column: "CodigoOptico");

            migrationBuilder.CreateIndex(
                name: "IX_gd$activo_EnPrestamo",
                table: "gd$activo",
                column: "EnPrestamo");

            migrationBuilder.CreateIndex(
                name: "IX_gd$activo_FechaCierre",
                table: "gd$activo",
                column: "FechaCierre");

            migrationBuilder.CreateIndex(
                name: "IX_gd$activo_FechaRetencionAC",
                table: "gd$activo",
                column: "FechaRetencionAC");

            migrationBuilder.CreateIndex(
                name: "IX_gd$activo_FechaRetencionAT",
                table: "gd$activo",
                column: "FechaRetencionAT");

            migrationBuilder.CreateIndex(
                name: "IX_gd$activo_Nombre",
                table: "gd$activo",
                column: "Nombre");

            migrationBuilder.AddForeignKey(
                name: "FK_gd$activo_gd$archivo_ArchivoOrigenId",
                table: "gd$activo",
                column: "ArchivoOrigenId",
                principalTable: "gd$archivo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_gd$activo_gd$archivo_ArchivoOrigenId",
                table: "gd$activo");

            migrationBuilder.DropIndex(
                name: "IX_gd$activo_Ampliado",
                table: "gd$activo");

            migrationBuilder.DropIndex(
                name: "IX_gd$activo_ArchivoOrigenId",
                table: "gd$activo");

            migrationBuilder.DropIndex(
                name: "IX_gd$activo_CodigoElectronico",
                table: "gd$activo");

            migrationBuilder.DropIndex(
                name: "IX_gd$activo_CodigoOptico",
                table: "gd$activo");

            migrationBuilder.DropIndex(
                name: "IX_gd$activo_EnPrestamo",
                table: "gd$activo");

            migrationBuilder.DropIndex(
                name: "IX_gd$activo_FechaCierre",
                table: "gd$activo");

            migrationBuilder.DropIndex(
                name: "IX_gd$activo_FechaRetencionAC",
                table: "gd$activo");

            migrationBuilder.DropIndex(
                name: "IX_gd$activo_FechaRetencionAT",
                table: "gd$activo");

            migrationBuilder.DropIndex(
                name: "IX_gd$activo_Nombre",
                table: "gd$activo");

            migrationBuilder.DropColumn(
                name: "ArchivoOrigenId",
                table: "gd$activo");

            migrationBuilder.DropColumn(
                name: "FechaRetencionAC",
                table: "gd$activo");

            migrationBuilder.DropColumn(
                name: "FechaRetencionAT",
                table: "gd$activo");

            migrationBuilder.AlterColumn<string>(
                name: "CodigoOptico",
                table: "gd$activo",
                type: "varchar(1024) CHARACTER SET utf8mb4",
                maxLength: 1024,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 512,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CodigoElectronico",
                table: "gd$activo",
                type: "varchar(1024) CHARACTER SET utf8mb4",
                maxLength: 1024,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 512,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ArchivoId",
                table: "gd$activo",
                type: "varchar(128) CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 128);
        }
    }
}
