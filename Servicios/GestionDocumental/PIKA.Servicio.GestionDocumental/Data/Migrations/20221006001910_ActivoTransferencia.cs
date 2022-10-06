using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.GestionDocumental.data.migrations
{
    public partial class ActivoTransferencia : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "gd$activotransferencia",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ActivoId = table.Column<string>(maxLength: 128, nullable: false),
                    TransferenciaId = table.Column<string>(maxLength: 128, nullable: false),
                    Declinado = table.Column<bool>(nullable: false, defaultValue: false),
                    MotivoDeclinado = table.Column<string>(maxLength: 500, nullable: true),
                    UsuarioIdDeclinadoId = table.Column<string>(maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$activotransferencia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_gd$activotransferencia_gd$activo_ActivoId",
                        column: x => x.ActivoId,
                        principalTable: "gd$activo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_gd$activotransferencia_gd$transferencia_TransferenciaId",
                        column: x => x.TransferenciaId,
                        principalTable: "gd$transferencia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_gd$activotransferencia_TransferenciaId",
                table: "gd$activotransferencia",
                column: "TransferenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$activotransferencia_UsuarioIdDeclinadoId",
                table: "gd$activotransferencia",
                column: "UsuarioIdDeclinadoId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$activotransferencia_ActivoId_TransferenciaId_Declinado",
                table: "gd$activotransferencia",
                columns: new[] { "ActivoId", "TransferenciaId", "Declinado" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "gd$activotransferencia");
        }
    }
}
