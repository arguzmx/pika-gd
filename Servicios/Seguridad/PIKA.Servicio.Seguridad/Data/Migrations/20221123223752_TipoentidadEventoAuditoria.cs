using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Seguridad.data.migrations
{
    public partial class TipoentidadEventoAuditoria : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TipoEntidad",
                table: "seguridad$eventosaud",
                maxLength: 64,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TipoEntidad",
                table: "seguridad$eventosaud");
        }
    }
}
