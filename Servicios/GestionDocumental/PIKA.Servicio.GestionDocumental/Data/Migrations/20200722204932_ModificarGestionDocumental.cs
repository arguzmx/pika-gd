using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.GestionDocumental.Data.Migrations
{
    public partial class ModificarGestionDocumental : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_gd$activo_declinado_gd$activo_ActivoId",
                table: "gd$activo_declinado");

            migrationBuilder.DropForeignKey(
                name: "FK_gd$activo_declinado_gd$transferencia_TransferenciaId",
                table: "gd$activo_declinado");

            migrationBuilder.DropForeignKey(
                name: "FK_gd$activo_prestamo_gd$activo_ActivoId",
                table: "gd$activo_prestamo");

            migrationBuilder.DropForeignKey(
                name: "FK_gd$activo_prestamo_gd$prestamo_PrestamoId",
                table: "gd$activo_prestamo");

            migrationBuilder.DropForeignKey(
                name: "FK_gd$activo_transferencia_gd$activo_ActivoId",
                table: "gd$activo_transferencia");

            migrationBuilder.DropForeignKey(
                name: "FK_gd$activo_transferencia_gd$transferencia_TransferenciaId",
                table: "gd$activo_transferencia");

            migrationBuilder.DropForeignKey(
                name: "FK_gd$comentario_prestamo_gd$prestamo_PrestamoId",
                table: "gd$comentario_prestamo");

            migrationBuilder.DropForeignKey(
                name: "FK_gd$espacio_estante_gd$estantes_EstanteId",
                table: "gd$espacio_estante");

            migrationBuilder.DropForeignKey(
                name: "FK_gd$evento_transferencia_gd$estado_transferencia_EstadoTransf~",
                table: "gd$evento_transferencia");

            migrationBuilder.DropForeignKey(
                name: "FK_gd$evento_transferencia_gd$transferencia_TransferenciaId",
                table: "gd$evento_transferencia");

            migrationBuilder.DropForeignKey(
                name: "FK_gd$transferencia_gd$estado_transferencia_EstadoTransferencia~",
                table: "gd$transferencia");

            migrationBuilder.DropTable(
                name: "gd$comentario_transferencia");

            migrationBuilder.DropPrimaryKey(
                name: "PK_gd$evento_transferencia",
                table: "gd$evento_transferencia");

            migrationBuilder.DropPrimaryKey(
                name: "PK_gd$estado_transferencia",
                table: "gd$estado_transferencia");

            migrationBuilder.DropPrimaryKey(
                name: "PK_gd$espacio_estante",
                table: "gd$espacio_estante");

            migrationBuilder.DropPrimaryKey(
                name: "PK_gd$comentario_prestamo",
                table: "gd$comentario_prestamo");

            migrationBuilder.DropPrimaryKey(
                name: "PK_gd$activo_transferencia",
                table: "gd$activo_transferencia");

            migrationBuilder.DropPrimaryKey(
                name: "PK_gd$activo_prestamo",
                table: "gd$activo_prestamo");

            migrationBuilder.DropPrimaryKey(
                name: "PK_gd$activo_declinado",
                table: "gd$activo_declinado");

            migrationBuilder.RenameTable(
                name: "gd$evento_transferencia",
                newName: "gd$eventotransferencia");

            migrationBuilder.RenameTable(
                name: "gd$estado_transferencia",
                newName: "gd$estadotransferencia");

            migrationBuilder.RenameTable(
                name: "gd$espacio_estante",
                newName: "gd$espacioestante");

            migrationBuilder.RenameTable(
                name: "gd$comentario_prestamo",
                newName: "gd$comentarioprestamo");

            migrationBuilder.RenameTable(
                name: "gd$activo_transferencia",
                newName: "gd$activotransferencia");

            migrationBuilder.RenameTable(
                name: "gd$activo_prestamo",
                newName: "gd$activoprestamo");

            migrationBuilder.RenameTable(
                name: "gd$activo_declinado",
                newName: "gd$activodeclinado");

            migrationBuilder.RenameIndex(
                name: "IX_gd$evento_transferencia_TransferenciaId",
                table: "gd$eventotransferencia",
                newName: "IX_gd$eventotransferencia_TransferenciaId");

            migrationBuilder.RenameIndex(
                name: "IX_gd$evento_transferencia_EstadoTransferenciaId",
                table: "gd$eventotransferencia",
                newName: "IX_gd$eventotransferencia_EstadoTransferenciaId");

            migrationBuilder.RenameIndex(
                name: "IX_gd$espacio_estante_EstanteId",
                table: "gd$espacioestante",
                newName: "IX_gd$espacioestante_EstanteId");

            migrationBuilder.RenameIndex(
                name: "IX_gd$comentario_prestamo_PrestamoId",
                table: "gd$comentarioprestamo",
                newName: "IX_gd$comentarioprestamo_PrestamoId");

            migrationBuilder.RenameIndex(
                name: "IX_gd$activo_transferencia_TransferenciaId",
                table: "gd$activotransferencia",
                newName: "IX_gd$activotransferencia_TransferenciaId");

            migrationBuilder.RenameIndex(
                name: "IX_gd$activo_prestamo_ActivoId",
                table: "gd$activoprestamo",
                newName: "IX_gd$activoprestamo_ActivoId");

            migrationBuilder.RenameIndex(
                name: "IX_gd$activo_declinado_TransferenciaId",
                table: "gd$activodeclinado",
                newName: "IX_gd$activodeclinado_TransferenciaId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_gd$eventotransferencia",
                table: "gd$eventotransferencia",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_gd$estadotransferencia",
                table: "gd$estadotransferencia",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_gd$espacioestante",
                table: "gd$espacioestante",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_gd$comentarioprestamo",
                table: "gd$comentarioprestamo",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_gd$activotransferencia",
                table: "gd$activotransferencia",
                columns: new[] { "ActivoId", "TransferenciaId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_gd$activoprestamo",
                table: "gd$activoprestamo",
                columns: new[] { "PrestamoId", "ActivoId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_gd$activodeclinado",
                table: "gd$activodeclinado",
                columns: new[] { "ActivoId", "TransferenciaId" });

            migrationBuilder.CreateTable(
                name: "gd$comentariotransferencia",
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
                    table.PrimaryKey("PK_gd$comentariotransferencia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_gd$comentariotransferencia_gd$transferencia_TransferenciaId",
                        column: x => x.TransferenciaId,
                        principalTable: "gd$transferencia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "gd$tipodisposiciondocumental",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$tipodisposiciondocumental", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "gd$tipovaloraciondocumental",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$tipovaloraciondocumental", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "gd$entradaclasificacion",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    ElementoClasificacionId = table.Column<string>(maxLength: 128, nullable: false),
                    Clave = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false),
                    Eliminada = table.Column<bool>(nullable: false, defaultValue: false),
                    Posicion = table.Column<int>(nullable: false),
                    MesesVigenciTramite = table.Column<int>(nullable: false),
                    MesesVigenciConcentracion = table.Column<int>(nullable: false),
                    MesesVigenciHistorico = table.Column<int>(nullable: false),
                    TipoDisposicionDocumentalId = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$entradaclasificacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_gd$entradaclasificacion_gd$elementoclasificacion_ElementoCla~",
                        column: x => x.ElementoClasificacionId,
                        principalTable: "gd$elementoclasificacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_gd$entradaclasificacion_gd$tipodisposiciondocumental_TipoDis~",
                        column: x => x.TipoDisposicionDocumentalId,
                        principalTable: "gd$tipodisposiciondocumental",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "gd$valoracionentradaclasificacion",
                columns: table => new
                {
                    EntradaClasificacionId = table.Column<string>(maxLength: 128, nullable: false),
                    TipoValoracionDocumentalId = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$valoracionentradaclasificacion", x => new { x.EntradaClasificacionId, x.TipoValoracionDocumentalId });
                    table.ForeignKey(
                        name: "FK_gd$valoracionentradaclasificacion_gd$entradaclasificacion_En~",
                        column: x => x.EntradaClasificacionId,
                        principalTable: "gd$entradaclasificacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_gd$valoracionentradaclasificacion_gd$tipovaloraciondocumenta~",
                        column: x => x.TipoValoracionDocumentalId,
                        principalTable: "gd$tipovaloraciondocumental",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_gd$comentariotransferencia_TransferenciaId",
                table: "gd$comentariotransferencia",
                column: "TransferenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$entradaclasificacion_ElementoClasificacionId",
                table: "gd$entradaclasificacion",
                column: "ElementoClasificacionId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$entradaclasificacion_TipoDisposicionDocumentalId",
                table: "gd$entradaclasificacion",
                column: "TipoDisposicionDocumentalId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$valoracionentradaclasificacion_TipoValoracionDocumentalId",
                table: "gd$valoracionentradaclasificacion",
                column: "TipoValoracionDocumentalId");

            migrationBuilder.AddForeignKey(
                name: "FK_gd$activodeclinado_gd$activo_ActivoId",
                table: "gd$activodeclinado",
                column: "ActivoId",
                principalTable: "gd$activo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_gd$activodeclinado_gd$transferencia_TransferenciaId",
                table: "gd$activodeclinado",
                column: "TransferenciaId",
                principalTable: "gd$transferencia",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_gd$activoprestamo_gd$activo_ActivoId",
                table: "gd$activoprestamo",
                column: "ActivoId",
                principalTable: "gd$activo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_gd$activoprestamo_gd$prestamo_PrestamoId",
                table: "gd$activoprestamo",
                column: "PrestamoId",
                principalTable: "gd$prestamo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_gd$activotransferencia_gd$activo_ActivoId",
                table: "gd$activotransferencia",
                column: "ActivoId",
                principalTable: "gd$activo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_gd$activotransferencia_gd$transferencia_TransferenciaId",
                table: "gd$activotransferencia",
                column: "TransferenciaId",
                principalTable: "gd$transferencia",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_gd$comentarioprestamo_gd$prestamo_PrestamoId",
                table: "gd$comentarioprestamo",
                column: "PrestamoId",
                principalTable: "gd$prestamo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_gd$espacioestante_gd$estantes_EstanteId",
                table: "gd$espacioestante",
                column: "EstanteId",
                principalTable: "gd$estantes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_gd$eventotransferencia_gd$estadotransferencia_EstadoTransfer~",
                table: "gd$eventotransferencia",
                column: "EstadoTransferenciaId",
                principalTable: "gd$estadotransferencia",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_gd$eventotransferencia_gd$transferencia_TransferenciaId",
                table: "gd$eventotransferencia",
                column: "TransferenciaId",
                principalTable: "gd$transferencia",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_gd$transferencia_gd$estadotransferencia_EstadoTransferenciaId",
                table: "gd$transferencia",
                column: "EstadoTransferenciaId",
                principalTable: "gd$estadotransferencia",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_gd$activodeclinado_gd$activo_ActivoId",
                table: "gd$activodeclinado");

            migrationBuilder.DropForeignKey(
                name: "FK_gd$activodeclinado_gd$transferencia_TransferenciaId",
                table: "gd$activodeclinado");

            migrationBuilder.DropForeignKey(
                name: "FK_gd$activoprestamo_gd$activo_ActivoId",
                table: "gd$activoprestamo");

            migrationBuilder.DropForeignKey(
                name: "FK_gd$activoprestamo_gd$prestamo_PrestamoId",
                table: "gd$activoprestamo");

            migrationBuilder.DropForeignKey(
                name: "FK_gd$activotransferencia_gd$activo_ActivoId",
                table: "gd$activotransferencia");

            migrationBuilder.DropForeignKey(
                name: "FK_gd$activotransferencia_gd$transferencia_TransferenciaId",
                table: "gd$activotransferencia");

            migrationBuilder.DropForeignKey(
                name: "FK_gd$comentarioprestamo_gd$prestamo_PrestamoId",
                table: "gd$comentarioprestamo");

            migrationBuilder.DropForeignKey(
                name: "FK_gd$espacioestante_gd$estantes_EstanteId",
                table: "gd$espacioestante");

            migrationBuilder.DropForeignKey(
                name: "FK_gd$eventotransferencia_gd$estadotransferencia_EstadoTransfer~",
                table: "gd$eventotransferencia");

            migrationBuilder.DropForeignKey(
                name: "FK_gd$eventotransferencia_gd$transferencia_TransferenciaId",
                table: "gd$eventotransferencia");

            migrationBuilder.DropForeignKey(
                name: "FK_gd$transferencia_gd$estadotransferencia_EstadoTransferenciaId",
                table: "gd$transferencia");

            migrationBuilder.DropTable(
                name: "gd$comentariotransferencia");

            migrationBuilder.DropTable(
                name: "gd$valoracionentradaclasificacion");

            migrationBuilder.DropTable(
                name: "gd$entradaclasificacion");

            migrationBuilder.DropTable(
                name: "gd$tipovaloraciondocumental");

            migrationBuilder.DropTable(
                name: "gd$tipodisposiciondocumental");

            migrationBuilder.DropPrimaryKey(
                name: "PK_gd$eventotransferencia",
                table: "gd$eventotransferencia");

            migrationBuilder.DropPrimaryKey(
                name: "PK_gd$estadotransferencia",
                table: "gd$estadotransferencia");

            migrationBuilder.DropPrimaryKey(
                name: "PK_gd$espacioestante",
                table: "gd$espacioestante");

            migrationBuilder.DropPrimaryKey(
                name: "PK_gd$comentarioprestamo",
                table: "gd$comentarioprestamo");

            migrationBuilder.DropPrimaryKey(
                name: "PK_gd$activotransferencia",
                table: "gd$activotransferencia");

            migrationBuilder.DropPrimaryKey(
                name: "PK_gd$activoprestamo",
                table: "gd$activoprestamo");

            migrationBuilder.DropPrimaryKey(
                name: "PK_gd$activodeclinado",
                table: "gd$activodeclinado");

            migrationBuilder.RenameTable(
                name: "gd$eventotransferencia",
                newName: "gd$evento_transferencia");

            migrationBuilder.RenameTable(
                name: "gd$estadotransferencia",
                newName: "gd$estado_transferencia");

            migrationBuilder.RenameTable(
                name: "gd$espacioestante",
                newName: "gd$espacio_estante");

            migrationBuilder.RenameTable(
                name: "gd$comentarioprestamo",
                newName: "gd$comentario_prestamo");

            migrationBuilder.RenameTable(
                name: "gd$activotransferencia",
                newName: "gd$activo_transferencia");

            migrationBuilder.RenameTable(
                name: "gd$activoprestamo",
                newName: "gd$activo_prestamo");

            migrationBuilder.RenameTable(
                name: "gd$activodeclinado",
                newName: "gd$activo_declinado");

            migrationBuilder.RenameIndex(
                name: "IX_gd$eventotransferencia_TransferenciaId",
                table: "gd$evento_transferencia",
                newName: "IX_gd$evento_transferencia_TransferenciaId");

            migrationBuilder.RenameIndex(
                name: "IX_gd$eventotransferencia_EstadoTransferenciaId",
                table: "gd$evento_transferencia",
                newName: "IX_gd$evento_transferencia_EstadoTransferenciaId");

            migrationBuilder.RenameIndex(
                name: "IX_gd$espacioestante_EstanteId",
                table: "gd$espacio_estante",
                newName: "IX_gd$espacio_estante_EstanteId");

            migrationBuilder.RenameIndex(
                name: "IX_gd$comentarioprestamo_PrestamoId",
                table: "gd$comentario_prestamo",
                newName: "IX_gd$comentario_prestamo_PrestamoId");

            migrationBuilder.RenameIndex(
                name: "IX_gd$activotransferencia_TransferenciaId",
                table: "gd$activo_transferencia",
                newName: "IX_gd$activo_transferencia_TransferenciaId");

            migrationBuilder.RenameIndex(
                name: "IX_gd$activoprestamo_ActivoId",
                table: "gd$activo_prestamo",
                newName: "IX_gd$activo_prestamo_ActivoId");

            migrationBuilder.RenameIndex(
                name: "IX_gd$activodeclinado_TransferenciaId",
                table: "gd$activo_declinado",
                newName: "IX_gd$activo_declinado_TransferenciaId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_gd$evento_transferencia",
                table: "gd$evento_transferencia",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_gd$estado_transferencia",
                table: "gd$estado_transferencia",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_gd$espacio_estante",
                table: "gd$espacio_estante",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_gd$comentario_prestamo",
                table: "gd$comentario_prestamo",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_gd$activo_transferencia",
                table: "gd$activo_transferencia",
                columns: new[] { "ActivoId", "TransferenciaId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_gd$activo_prestamo",
                table: "gd$activo_prestamo",
                columns: new[] { "PrestamoId", "ActivoId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_gd$activo_declinado",
                table: "gd$activo_declinado",
                columns: new[] { "ActivoId", "TransferenciaId" });

            migrationBuilder.CreateTable(
                name: "gd$comentario_transferencia",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false),
                    Comentario = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", maxLength: 2048, nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Publico = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    TransferenciaId = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false),
                    UsuarioId = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_gd$comentario_transferencia_TransferenciaId",
                table: "gd$comentario_transferencia",
                column: "TransferenciaId");

            migrationBuilder.AddForeignKey(
                name: "FK_gd$activo_declinado_gd$activo_ActivoId",
                table: "gd$activo_declinado",
                column: "ActivoId",
                principalTable: "gd$activo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_gd$activo_declinado_gd$transferencia_TransferenciaId",
                table: "gd$activo_declinado",
                column: "TransferenciaId",
                principalTable: "gd$transferencia",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_gd$activo_prestamo_gd$activo_ActivoId",
                table: "gd$activo_prestamo",
                column: "ActivoId",
                principalTable: "gd$activo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_gd$activo_prestamo_gd$prestamo_PrestamoId",
                table: "gd$activo_prestamo",
                column: "PrestamoId",
                principalTable: "gd$prestamo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_gd$activo_transferencia_gd$activo_ActivoId",
                table: "gd$activo_transferencia",
                column: "ActivoId",
                principalTable: "gd$activo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_gd$activo_transferencia_gd$transferencia_TransferenciaId",
                table: "gd$activo_transferencia",
                column: "TransferenciaId",
                principalTable: "gd$transferencia",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_gd$comentario_prestamo_gd$prestamo_PrestamoId",
                table: "gd$comentario_prestamo",
                column: "PrestamoId",
                principalTable: "gd$prestamo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_gd$espacio_estante_gd$estantes_EstanteId",
                table: "gd$espacio_estante",
                column: "EstanteId",
                principalTable: "gd$estantes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_gd$evento_transferencia_gd$estado_transferencia_EstadoTransf~",
                table: "gd$evento_transferencia",
                column: "EstadoTransferenciaId",
                principalTable: "gd$estado_transferencia",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_gd$evento_transferencia_gd$transferencia_TransferenciaId",
                table: "gd$evento_transferencia",
                column: "TransferenciaId",
                principalTable: "gd$transferencia",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_gd$transferencia_gd$estado_transferencia_EstadoTransferencia~",
                table: "gd$transferencia",
                column: "EstadoTransferenciaId",
                principalTable: "gd$estado_transferencia",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
