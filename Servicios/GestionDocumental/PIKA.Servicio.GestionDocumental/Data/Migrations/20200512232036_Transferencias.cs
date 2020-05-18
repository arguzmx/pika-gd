using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.GestionDocumental.Data.Migrations
{
    public partial class Transferencias : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Id",
                table: "gd$ampliacion");

            migrationBuilder.CreateTable(
                name: "gd$estado_transferencia",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$estado_transferencia", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "gd$transferencia",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false),
                    FechaCreacion = table.Column<DateTime>(nullable: false),
                    EstadoTransferenciaId = table.Column<string>(maxLength: 128, nullable: false),
                    ArchivoOrigenId = table.Column<string>(maxLength: 128, nullable: false),
                    ArchivoDestinoId = table.Column<string>(maxLength: 128, nullable: false),
                    UsuarioId = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$transferencia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_gd$transferencia_gd$archivo_ArchivoDestinoId",
                        column: x => x.ArchivoDestinoId,
                        principalTable: "gd$archivo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_gd$transferencia_gd$archivo_ArchivoOrigenId",
                        column: x => x.ArchivoOrigenId,
                        principalTable: "gd$archivo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_gd$transferencia_gd$estado_transferencia_EstadoTransferencia~",
                        column: x => x.EstadoTransferenciaId,
                        principalTable: "gd$estado_transferencia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "gd$activo_declinado",
                columns: table => new
                {
                    ActivoId = table.Column<string>(maxLength: 128, nullable: false),
                    TransferenciaId = table.Column<string>(maxLength: 128, nullable: false),
                    Motivo = table.Column<string>(maxLength: 2048, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$activo_declinado", x => new { x.ActivoId, x.TransferenciaId });
                    table.ForeignKey(
                        name: "FK_gd$activo_declinado_gd$activo_ActivoId",
                        column: x => x.ActivoId,
                        principalTable: "gd$activo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_gd$activo_declinado_gd$transferencia_TransferenciaId",
                        column: x => x.TransferenciaId,
                        principalTable: "gd$transferencia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "gd$activo_transferencia",
                columns: table => new
                {
                    ActivoId = table.Column<string>(maxLength: 128, nullable: false),
                    TransferenciaId = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$activo_transferencia", x => new { x.ActivoId, x.TransferenciaId });
                    table.ForeignKey(
                        name: "FK_gd$activo_transferencia_gd$activo_ActivoId",
                        column: x => x.ActivoId,
                        principalTable: "gd$activo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_gd$activo_transferencia_gd$transferencia_TransferenciaId",
                        column: x => x.TransferenciaId,
                        principalTable: "gd$transferencia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "gd$comentario_transferencia",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    TransferenciaId = table.Column<string>(maxLength: 128, nullable: false),
                    UsuarioId = table.Column<string>(maxLength: 128, nullable: false),
                    Fecha = table.Column<DateTime>(nullable: false),
                    Comentario = table.Column<string>(maxLength: 2048, nullable: false),
                    Publico = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$comentario_transferencia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_gd$comentario_transferencia_gd$transferencia_TransferenciaId",
                        column: x => x.TransferenciaId,
                        principalTable: "gd$transferencia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "gd$evento_transferencia",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    TransferenciaId = table.Column<string>(maxLength: 128, nullable: false),
                    Fecha = table.Column<DateTime>(nullable: false),
                    EstadoTransferenciaId = table.Column<string>(maxLength: 128, nullable: false),
                    Comentario = table.Column<string>(maxLength: 2048, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$evento_transferencia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_gd$evento_transferencia_gd$estado_transferencia_EstadoTransf~",
                        column: x => x.EstadoTransferenciaId,
                        principalTable: "gd$estado_transferencia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_gd$evento_transferencia_gd$transferencia_TransferenciaId",
                        column: x => x.TransferenciaId,
                        principalTable: "gd$transferencia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_gd$activo_declinado_TransferenciaId",
                table: "gd$activo_declinado",
                column: "TransferenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$activo_transferencia_TransferenciaId",
                table: "gd$activo_transferencia",
                column: "TransferenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$comentario_transferencia_TransferenciaId",
                table: "gd$comentario_transferencia",
                column: "TransferenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$evento_transferencia_EstadoTransferenciaId",
                table: "gd$evento_transferencia",
                column: "EstadoTransferenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$evento_transferencia_TransferenciaId",
                table: "gd$evento_transferencia",
                column: "TransferenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$transferencia_ArchivoDestinoId",
                table: "gd$transferencia",
                column: "ArchivoDestinoId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$transferencia_ArchivoOrigenId",
                table: "gd$transferencia",
                column: "ArchivoOrigenId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$transferencia_EstadoTransferenciaId",
                table: "gd$transferencia",
                column: "EstadoTransferenciaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "gd$activo_declinado");

            migrationBuilder.DropTable(
                name: "gd$activo_transferencia");

            migrationBuilder.DropTable(
                name: "gd$comentario_transferencia");

            migrationBuilder.DropTable(
                name: "gd$evento_transferencia");

            migrationBuilder.DropTable(
                name: "gd$transferencia");

            migrationBuilder.DropTable(
                name: "gd$estado_transferencia");

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "gd$ampliacion",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);
        }
    }
}
