using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.GestionDocumental.data.migrations
{
    public partial class EMActivosDeclinados : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "gd$activodeclinado");

            migrationBuilder.DropTable(
                name: "gd$activotransferencia");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "gd$activodeclinado",
                columns: table => new
                {
                    ActivoId = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false),
                    TransferenciaId = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false),
                    Motivo = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", maxLength: 2048, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$activodeclinado", x => new { x.ActivoId, x.TransferenciaId });
                    table.ForeignKey(
                        name: "FK_gd$activodeclinado_gd$activo_ActivoId",
                        column: x => x.ActivoId,
                        principalTable: "gd$activo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_gd$activodeclinado_gd$transferencia_TransferenciaId",
                        column: x => x.TransferenciaId,
                        principalTable: "gd$transferencia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "gd$activotransferencia",
                columns: table => new
                {
                    ActivoId = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false),
                    TransferenciaId = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$activotransferencia", x => new { x.ActivoId, x.TransferenciaId });
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
                name: "IX_gd$activodeclinado_TransferenciaId",
                table: "gd$activodeclinado",
                column: "TransferenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$activotransferencia_TransferenciaId",
                table: "gd$activotransferencia",
                column: "TransferenciaId");
        }
    }
}
