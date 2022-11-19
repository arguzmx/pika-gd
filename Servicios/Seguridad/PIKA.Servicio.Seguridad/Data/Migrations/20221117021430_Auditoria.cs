using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Seguridad.Data.Migrations
{
    public partial class Auditoria : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "PermisosDisponibles",
                table: "seguridad$moduloaplicacion",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "bigint unsigned",
                oldMaxLength: 128);

            migrationBuilder.CreateTable(
                name: "seguridad$eventosaud",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Fecha = table.Column<DateTime>(nullable: false),
                    DireccionRed = table.Column<string>(maxLength: 128, nullable: false),
                    IdSesion = table.Column<string>(maxLength: 128, nullable: true),
                    UsuarioId = table.Column<string>(maxLength: 128, nullable: false),
                    DominioId = table.Column<string>(maxLength: 128, nullable: false),
                    UAId = table.Column<string>(maxLength: 128, nullable: false),
                    Exitoso = table.Column<bool>(nullable: false),
                    EsError = table.Column<bool>(nullable: false),
                    FuenteEventoId = table.Column<string>(maxLength: 128, nullable: false),
                    ModuloId = table.Column<string>(maxLength: 128, nullable: false),
                    TipoEvento = table.Column<int>(nullable: false),
                    Texto = table.Column<string>(maxLength: 2048, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_seguridad$eventosaud", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "seguridad$eventosaudconf",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    DominioId = table.Column<string>(nullable: false),
                    UAId = table.Column<string>(maxLength: 128, nullable: false),
                    FuenteEventoId = table.Column<string>(maxLength: 128, nullable: false),
                    ModuloId = table.Column<string>(maxLength: 128, nullable: false),
                    TipoEvento = table.Column<int>(nullable: false),
                    Auditable = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_seguridad$eventosaudconf", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "seguridad$tipoeventosaud",
                columns: table => new
                {
                    FuenteEventoId = table.Column<string>(maxLength: 128, nullable: false),
                    ModuloId = table.Column<string>(maxLength: 128, nullable: false),
                    TipoEvento = table.Column<int>(nullable: false),
                    Desripción = table.Column<string>(maxLength: 500, nullable: false),
                    PlantillaEvento = table.Column<string>(nullable: true)
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
                name: "IX_seguridad$eventosaud_UsuarioId_Fecha_FuenteEventoId_ModuloId",
                table: "seguridad$eventosaud",
                columns: new[] { "UsuarioId", "Fecha", "FuenteEventoId", "ModuloId" });

            migrationBuilder.CreateIndex(
                name: "IX_seguridad$eventosaudconf_DominioId_UAId",
                table: "seguridad$eventosaudconf",
                columns: new[] { "DominioId", "UAId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "seguridad$eventosaud");

            migrationBuilder.DropTable(
                name: "seguridad$eventosaudconf");

            migrationBuilder.DropTable(
                name: "seguridad$tipoeventosaud");

            migrationBuilder.AlterColumn<ulong>(
                name: "PermisosDisponibles",
                table: "seguridad$moduloaplicacion",
                type: "bigint unsigned",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(int));
        }
    }
}
