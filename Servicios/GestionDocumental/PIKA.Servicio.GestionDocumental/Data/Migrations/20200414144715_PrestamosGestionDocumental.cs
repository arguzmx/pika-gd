using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.GestionDocumental.Data.Migrations
{
    public partial class PrestamosGestionDocumental : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_gd$activo_gd$archivo_ArchivoId",
                table: "gd$activo");

            migrationBuilder.DropForeignKey(
                name: "FK_gd$activo_gd$elementoclasificacion_ElementoClasificacionId",
                table: "gd$activo");

            migrationBuilder.DropForeignKey(
                name: "FK_gd$asunto_gd$activo_ActivoId",
                table: "gd$asunto");

            migrationBuilder.AlterColumn<string>(
                name: "ActivoId",
                table: "gd$asunto",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(128) CHARACTER SET utf8mb4",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ElementoClasificacionId",
                table: "gd$activo",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(128) CHARACTER SET utf8mb4",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ArchivoId",
                table: "gd$activo",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(128) CHARACTER SET utf8mb4",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "gd$prestamo",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Eliminada = table.Column<bool>(nullable: false, defaultValue: false),
                    ArchivoId = table.Column<string>(nullable: false),
                    FechaCreacion = table.Column<DateTime>(nullable: false),
                    FechaProgramadaDevolucion = table.Column<DateTime>(nullable: false),
                    FechaDevolucion = table.Column<DateTime>(nullable: true),
                    TieneDevolucionesParciales = table.Column<bool>(nullable: false, defaultValue: false),
                    Folio = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$prestamo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_gd$prestamo_gd$archivo_ArchivoId",
                        column: x => x.ArchivoId,
                        principalTable: "gd$archivo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "gd$activo_prestamo",
                columns: table => new
                {
                    PrestamoId = table.Column<string>(maxLength: 128, nullable: false),
                    ActivoId = table.Column<string>(maxLength: 128, nullable: false),
                    Devuelto = table.Column<bool>(nullable: false, defaultValue: false),
                    FechaDevolucion = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$activo_prestamo", x => new { x.PrestamoId, x.ActivoId });
                    table.ForeignKey(
                        name: "FK_gd$activo_prestamo_gd$activo_ActivoId",
                        column: x => x.ActivoId,
                        principalTable: "gd$activo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_gd$activo_prestamo_gd$prestamo_PrestamoId",
                        column: x => x.PrestamoId,
                        principalTable: "gd$prestamo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "gd$comentario_prestamo",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Fecha = table.Column<DateTime>(nullable: false),
                    Comentario = table.Column<string>(maxLength: 2048, nullable: false),
                    PrestamoId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$comentario_prestamo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_gd$comentario_prestamo_gd$prestamo_PrestamoId",
                        column: x => x.PrestamoId,
                        principalTable: "gd$prestamo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_gd$activo_prestamo_ActivoId",
                table: "gd$activo_prestamo",
                column: "ActivoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_gd$comentario_prestamo_PrestamoId",
                table: "gd$comentario_prestamo",
                column: "PrestamoId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$prestamo_ArchivoId",
                table: "gd$prestamo",
                column: "ArchivoId");

            migrationBuilder.AddForeignKey(
                name: "FK_gd$activo_gd$archivo_ArchivoId",
                table: "gd$activo",
                column: "ArchivoId",
                principalTable: "gd$archivo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_gd$activo_gd$elementoclasificacion_ElementoClasificacionId",
                table: "gd$activo",
                column: "ElementoClasificacionId",
                principalTable: "gd$elementoclasificacion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_gd$asunto_gd$activo_ActivoId",
                table: "gd$asunto",
                column: "ActivoId",
                principalTable: "gd$activo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_gd$activo_gd$archivo_ArchivoId",
                table: "gd$activo");

            migrationBuilder.DropForeignKey(
                name: "FK_gd$activo_gd$elementoclasificacion_ElementoClasificacionId",
                table: "gd$activo");

            migrationBuilder.DropForeignKey(
                name: "FK_gd$asunto_gd$activo_ActivoId",
                table: "gd$asunto");

            migrationBuilder.DropTable(
                name: "gd$activo_prestamo");

            migrationBuilder.DropTable(
                name: "gd$comentario_prestamo");

            migrationBuilder.DropTable(
                name: "gd$prestamo");

            migrationBuilder.AlterColumn<string>(
                name: "ActivoId",
                table: "gd$asunto",
                type: "varchar(128) CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "ElementoClasificacionId",
                table: "gd$activo",
                type: "varchar(128) CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "ArchivoId",
                table: "gd$activo",
                type: "varchar(128) CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddForeignKey(
                name: "FK_gd$activo_gd$archivo_ArchivoId",
                table: "gd$activo",
                column: "ArchivoId",
                principalTable: "gd$archivo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_gd$activo_gd$elementoclasificacion_ElementoClasificacionId",
                table: "gd$activo",
                column: "ElementoClasificacionId",
                principalTable: "gd$elementoclasificacion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_gd$asunto_gd$activo_ActivoId",
                table: "gd$asunto",
                column: "ActivoId",
                principalTable: "gd$activo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
