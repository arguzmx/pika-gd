using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Contenido.Data.Migrations
{
    public partial class PermisosPuntoMontajeUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_contenido$permpmontaje_contenido$pmontaje_PuntoMontajeIdId",
                table: "contenido$permpmontaje");

            migrationBuilder.DropIndex(
                name: "IX_contenido$permpmontaje_PuntoMontajeIdId",
                table: "contenido$permpmontaje");

            migrationBuilder.DropColumn(
                name: "PuntoMontajeIdId",
                table: "contenido$permpmontaje");

            migrationBuilder.AddColumn<string>(
                name: "PuntoMontajeId",
                table: "contenido$permpmontaje",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_contenido$permpmontaje_PuntoMontajeId",
                table: "contenido$permpmontaje",
                column: "PuntoMontajeId");

            migrationBuilder.AddForeignKey(
                name: "FK_contenido$permpmontaje_contenido$pmontaje_PuntoMontajeId",
                table: "contenido$permpmontaje",
                column: "PuntoMontajeId",
                principalTable: "contenido$pmontaje",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_contenido$permpmontaje_contenido$pmontaje_PuntoMontajeId",
                table: "contenido$permpmontaje");

            migrationBuilder.DropIndex(
                name: "IX_contenido$permpmontaje_PuntoMontajeId",
                table: "contenido$permpmontaje");

            migrationBuilder.DropColumn(
                name: "PuntoMontajeId",
                table: "contenido$permpmontaje");

            migrationBuilder.AddColumn<string>(
                name: "PuntoMontajeIdId",
                table: "contenido$permpmontaje",
                type: "varchar(128) CHARACTER SET utf8mb4",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_contenido$permpmontaje_PuntoMontajeIdId",
                table: "contenido$permpmontaje",
                column: "PuntoMontajeIdId");

            migrationBuilder.AddForeignKey(
                name: "FK_contenido$permpmontaje_contenido$pmontaje_PuntoMontajeIdId",
                table: "contenido$permpmontaje",
                column: "PuntoMontajeIdId",
                principalTable: "contenido$pmontaje",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
