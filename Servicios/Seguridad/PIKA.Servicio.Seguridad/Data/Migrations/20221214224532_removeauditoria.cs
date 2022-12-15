using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Seguridad.data.migrations
{
    public partial class removeauditoria : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "seguridad$eventosaud");

            migrationBuilder.DropTable(
                name: "seguridad$eventosaudconf");

            migrationBuilder.DropTable(
                name: "seguridad$tipoeventosaud");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "seguridad$eventosaud",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false),
                    Delta = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    DireccionRed = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false),
                    DominioId = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false),
                    EsError = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Exitoso = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FuenteEventoId = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false),
                    IdEntidad = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: true),
                    IdSesion = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: true),
                    ModuloId = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false),
                    Texto = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", maxLength: 2048, nullable: true),
                    TipoEntidad = table.Column<string>(type: "varchar(64) CHARACTER SET utf8mb4", maxLength: 64, nullable: true),
                    TipoEvento = table.Column<int>(type: "int", nullable: false),
                    TipoFalla = table.Column<int>(type: "int", nullable: true),
                    UAId = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false),
                    UsuarioId = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_seguridad$eventosaud", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "seguridad$eventosaudconf",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false),
                    Auditable = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DominioId = table.Column<string>(type: "varchar(255) CHARACTER SET utf8mb4", nullable: false),
                    FuenteEventoId = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false),
                    ModuloId = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false),
                    TipoEvento = table.Column<int>(type: "int", nullable: false),
                    UAId = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_seguridad$eventosaudconf", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "seguridad$tipoeventosaud",
                columns: table => new
                {
                    FuenteEventoId = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false),
                    TipoEvento = table.Column<int>(type: "int", nullable: false),
                    ModuloId = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false),
                    Desripción = table.Column<string>(type: "varchar(500) CHARACTER SET utf8mb4", maxLength: 500, nullable: false),
                    PlantillaEvento = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_seguridad$tipoeventosaud", x => new { x.FuenteEventoId, x.TipoEvento, x.ModuloId });
                });

            migrationBuilder.CreateIndex(
                name: "IX_seguridad$eventosaud_DominioId_UAId",
                table: "seguridad$eventosaud",
                columns: new[] { "DominioId", "UAId" });

            migrationBuilder.CreateIndex(
                name: "IX_seguridad$eventosaud_UsuarioId_Fecha_FuenteEventoId_ModuloId~",
                table: "seguridad$eventosaud",
                columns: new[] { "UsuarioId", "Fecha", "FuenteEventoId", "ModuloId", "Exitoso" });

            migrationBuilder.CreateIndex(
                name: "IX_seguridad$eventosaudconf_DominioId_UAId",
                table: "seguridad$eventosaudconf",
                columns: new[] { "DominioId", "UAId" });
        }
    }
}
