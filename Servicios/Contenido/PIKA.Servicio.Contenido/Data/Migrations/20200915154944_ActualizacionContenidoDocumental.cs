using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Contenido.data.Migrations
{
    public partial class ActualizacionContenidoDocumental : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrigenId",
                table: "contenido$elemento");

            migrationBuilder.DropColumn(
                name: "TipoOrigenId",
                table: "contenido$elemento");

            migrationBuilder.AddColumn<string>(
                name: "PuntoMontajeId",
                table: "contenido$elemento",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_contenido$elemento_PuntoMontajeId",
                table: "contenido$elemento",
                column: "PuntoMontajeId");

            migrationBuilder.AddForeignKey(
                name: "FK_contenido$elemento_contenido$pmontaje_PuntoMontajeId",
                table: "contenido$elemento",
                column: "PuntoMontajeId",
                principalTable: "contenido$pmontaje",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_contenido$elemento_contenido$pmontaje_PuntoMontajeId",
                table: "contenido$elemento");

            migrationBuilder.DropIndex(
                name: "IX_contenido$elemento_PuntoMontajeId",
                table: "contenido$elemento");

            migrationBuilder.DropColumn(
                name: "PuntoMontajeId",
                table: "contenido$elemento");

            migrationBuilder.AddColumn<string>(
                name: "OrigenId",
                table: "contenido$elemento",
                type: "varchar(128) CHARACTER SET utf8mb4",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TipoOrigenId",
                table: "contenido$elemento",
                type: "varchar(128) CHARACTER SET utf8mb4",
                maxLength: 128,
                nullable: false,
                defaultValue: "");
        }
    }
}
