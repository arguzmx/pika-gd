using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Seguridad.Data.Migrations
{
    public partial class ActualizacionPermisosApp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "seguridad$permisosapl",
                columns: table => new
                {
                    DominioId = table.Column<string>(maxLength: 128, nullable: false),
                    AplicacionId = table.Column<string>(maxLength: 128, nullable: false),
                    ModuloId = table.Column<string>(maxLength: 128, nullable: false),
                    TipoEntidadAcceso = table.Column<string>(maxLength: 128, nullable: false),
                    EntidadAccesoId = table.Column<string>(maxLength: 128, nullable: false),
                    NegarAcceso = table.Column<bool>(nullable: false),
                    Leer = table.Column<bool>(nullable: false),
                    Escribir = table.Column<bool>(nullable: false),
                    Eliminar = table.Column<bool>(nullable: false),
                    Admin = table.Column<bool>(nullable: false),
                    Ejecutar = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_seguridad$permisosapl", x => new { x.DominioId, x.AplicacionId, x.ModuloId, x.EntidadAccesoId, x.TipoEntidadAcceso });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "seguridad$permisosapl");
        }
    }
}
