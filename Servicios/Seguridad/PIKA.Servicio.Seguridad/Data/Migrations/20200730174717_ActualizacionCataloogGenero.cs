using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Seguridad.data.Migrations
{
    public partial class ActualizacionCataloogGenero : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                       name: "FK_seguridad$usuarioprops_seguridad$generousuario_generoid",
                       table: "seguridad$usuarioprops");
    
            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "seguridad$generousuario",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255) CHARACTER SET utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "FK_seguridad$usuarioprops_seguridad$generousuario_generoid",
                table: "seguridad$usuarioprops",
                column: "generoid",
                principalTable: "seguridad$generousuario",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_seguridad$usuarioprops_seguridad$generousuario_generoid",
                table: "seguridad$usuarioprops");


            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "seguridad$generousuario",
                type: "varchar(255) CHARACTER SET utf8mb4",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 128);

            migrationBuilder.AddForeignKey(
                name: "FK_seguridad$usuarioprops_seguridad$generousuario_generoid",
                table: "seguridad$usuarioprops",
                column: "generoid",
                principalTable: "seguridad$generousuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
