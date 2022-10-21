using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.GestionDocumental.data.migrations
{
    public partial class DropEventoTx : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "gd$eventotransferencia");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "gd$eventotransferencia",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false),
                    Comentario = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", maxLength: 2048, nullable: true),
                    EstadoTransferenciaId = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    TransferenciaId = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false),
                    UsuarioId = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$eventotransferencia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_gd$eventotransferencia_gd$estadotransferencia_EstadoTransfer~",
                        column: x => x.EstadoTransferenciaId,
                        principalTable: "gd$estadotransferencia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_gd$eventotransferencia_gd$transferencia_TransferenciaId",
                        column: x => x.TransferenciaId,
                        principalTable: "gd$transferencia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_gd$eventotransferencia_EstadoTransferenciaId",
                table: "gd$eventotransferencia",
                column: "EstadoTransferenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$eventotransferencia_TransferenciaId",
                table: "gd$eventotransferencia",
                column: "TransferenciaId");
        }
    }
}
