using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Organizacion.Data.migrations
{
    public partial class EstadoPaisId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_org$estado_org$pais_PaisId",
                table: "org$estado");

            migrationBuilder.AlterColumn<string>(
                name: "PaisId",
                table: "org$estado",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(128) CHARACTER SET utf8mb4",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_org$estado_org$pais_PaisId",
                table: "org$estado",
                column: "PaisId",
                principalTable: "org$pais",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_org$estado_org$pais_PaisId",
                table: "org$estado");

            migrationBuilder.AlterColumn<string>(
                name: "PaisId",
                table: "org$estado",
                type: "varchar(128) CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 128);

            migrationBuilder.AddForeignKey(
                name: "FK_org$estado_org$pais_PaisId",
                table: "org$estado",
                column: "PaisId",
                principalTable: "org$pais",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
