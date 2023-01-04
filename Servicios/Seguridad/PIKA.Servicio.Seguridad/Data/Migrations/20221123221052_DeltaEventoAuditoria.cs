using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Seguridad.data.migrations
{
    public partial class DeltaEventoAuditoria : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_seguridad$eventosaud_UsuarioId_Fecha_FuenteEventoId_ModuloId",
                table: "seguridad$eventosaud");

            migrationBuilder.AddColumn<string>(
                name: "Delta",
                table: "seguridad$eventosaud",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdEntidad",
                table: "seguridad$eventosaud",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TipoFalla",
                table: "seguridad$eventosaud",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_seguridad$eventosaud_UsuarioId_Fecha_FuenteEventoId_ModuloId~",
                table: "seguridad$eventosaud",
                columns: new[] { "UsuarioId", "Fecha", "FuenteEventoId", "ModuloId", "Exitoso" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_seguridad$eventosaud_UsuarioId_Fecha_FuenteEventoId_ModuloId~",
                table: "seguridad$eventosaud");

            migrationBuilder.DropColumn(
                name: "Delta",
                table: "seguridad$eventosaud");

            migrationBuilder.DropColumn(
                name: "IdEntidad",
                table: "seguridad$eventosaud");

            migrationBuilder.DropColumn(
                name: "TipoFalla",
                table: "seguridad$eventosaud");

            migrationBuilder.CreateIndex(
                name: "IX_seguridad$eventosaud_UsuarioId_Fecha_FuenteEventoId_ModuloId",
                table: "seguridad$eventosaud",
                columns: new[] { "UsuarioId", "Fecha", "FuenteEventoId", "ModuloId" });
        }
    }
}
