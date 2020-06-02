using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Organizacion.Data.migrations
{
    public partial class ActualizacionGeneral001 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_org$ou_org$dominio_DominioId",
                table: "org$ou");

            migrationBuilder.DropForeignKey(
                name: "FK_org$rol_org$rol_RolPadreId",
                table: "org$rol");

            migrationBuilder.DropIndex(
                name: "IX_org$rol_RolPadreId",
                table: "org$rol");

            migrationBuilder.DropColumn(
                name: "RolPadreId",
                table: "org$rol");

            migrationBuilder.DropColumn(
                name: "Valor",
                table: "org$pais");

            migrationBuilder.DropColumn(
                name: "Valor",
                table: "org$estado");

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "org$rol",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(500) CHARACTER SET utf8mb4",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Nombre",
                table: "org$pais",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "DominioId",
                table: "org$ou",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(128) CHARACTER SET utf8mb4",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Nombre",
                table: "org$estado",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "Eliminada",
                table: "org$dominio",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "org$direccion_postal",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200) CHARACTER SET utf8mb4",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Calle",
                table: "org$direccion_postal",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200) CHARACTER SET utf8mb4",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_org$ou_org$dominio_DominioId",
                table: "org$ou",
                column: "DominioId",
                principalTable: "org$dominio",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_org$ou_org$dominio_DominioId",
                table: "org$ou");

            migrationBuilder.DropColumn(
                name: "Nombre",
                table: "org$pais");

            migrationBuilder.DropColumn(
                name: "Nombre",
                table: "org$estado");

            migrationBuilder.DropColumn(
                name: "Eliminada",
                table: "org$dominio");

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "org$rol",
                type: "varchar(500) CHARACTER SET utf8mb4",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 500);

            migrationBuilder.AddColumn<string>(
                name: "RolPadreId",
                table: "org$rol",
                type: "varchar(128) CHARACTER SET utf8mb4",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Valor",
                table: "org$pais",
                type: "varchar(200) CHARACTER SET utf8mb4",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "DominioId",
                table: "org$ou",
                type: "varchar(128) CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 128);

            migrationBuilder.AddColumn<string>(
                name: "Valor",
                table: "org$estado",
                type: "varchar(200) CHARACTER SET utf8mb4",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "org$direccion_postal",
                type: "varchar(200) CHARACTER SET utf8mb4",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Calle",
                table: "org$direccion_postal",
                type: "varchar(200) CHARACTER SET utf8mb4",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 200);

            migrationBuilder.CreateIndex(
                name: "IX_org$rol_RolPadreId",
                table: "org$rol",
                column: "RolPadreId");

            migrationBuilder.AddForeignKey(
                name: "FK_org$ou_org$dominio_DominioId",
                table: "org$ou",
                column: "DominioId",
                principalTable: "org$dominio",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_org$rol_org$rol_RolPadreId",
                table: "org$rol",
                column: "RolPadreId",
                principalTable: "org$rol",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
