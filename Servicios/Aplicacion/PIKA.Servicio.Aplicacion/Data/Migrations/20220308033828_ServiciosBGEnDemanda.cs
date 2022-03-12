using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.AplicacionPlugin.Data.Migrations
{
    public partial class ServiciosBGEnDemanda : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "aplicacion$ondemandtarea",
                columns: table => new
                {
                    Id = table.Column<Guid>(maxLength: 128, nullable: false),
                    FechaCreacion = table.Column<DateTime>(nullable: false),
                    FechaEjecucion = table.Column<DateTime>(nullable: true),
                    Completada = table.Column<bool>(nullable: false),
                    UsuarioId = table.Column<string>(maxLength: 128, nullable: false),
                    DominioId = table.Column<string>(maxLength: 128, nullable: false),
                    TenantId = table.Column<string>(maxLength: 128, nullable: false),
                    NombreEnsamblado = table.Column<string>(maxLength: 250, nullable: false),
                    TareaProcesoId = table.Column<string>(maxLength: 250, nullable: false),
                    InputPayload = table.Column<string>(maxLength: 2000, nullable: true),
                    OutputPayload = table.Column<string>(maxLength: 2000, nullable: true),
                    Prioridad = table.Column<int>(nullable: false),
                    Error = table.Column<string>(maxLength: 500, nullable: true),
                    URLRecoleccion = table.Column<string>(maxLength: 250, nullable: false),
                    FechaCaducidad = table.Column<DateTime>(nullable: true),
                    TipoRespuesta = table.Column<int>(nullable: false),
                    Recogida = table.Column<bool>(nullable: false),
                    HorasCaducidad = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_aplicacion$ondemandtarea", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "aplicacion$ondemandtarea");
        }
    }
}
