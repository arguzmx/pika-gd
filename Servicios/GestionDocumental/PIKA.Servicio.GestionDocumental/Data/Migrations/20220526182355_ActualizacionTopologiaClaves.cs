using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.GestionDocumental.Data.Migrations
{
    public partial class ActualizacionTopologiaClaves : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_gd$zonasalmacen_Clave",
                table: "gd$zonasalmacen");

            migrationBuilder.DropIndex(
                name: "IX_gd$posalmacen_Clave",
                table: "gd$posalmacen");

            migrationBuilder.DropIndex(
                name: "IX_gd$contalmacen_Clave",
                table: "gd$contalmacen");

            migrationBuilder.DropColumn(
                name: "Clave",
                table: "gd$zonasalmacen");

            migrationBuilder.DropColumn(
                name: "Clave",
                table: "gd$posalmacen");

            migrationBuilder.DropColumn(
                name: "Clave",
                table: "gd$contalmacen");

            migrationBuilder.AddColumn<decimal>(
                name: "IncrementoContenedor",
                table: "gd$posalmacen",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Localizacion",
                table: "gd$posalmacen",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Ubicacion",
                table: "gd$almacen",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IncrementoContenedor",
                table: "gd$posalmacen");

            migrationBuilder.DropColumn(
                name: "Localizacion",
                table: "gd$posalmacen");

            migrationBuilder.AddColumn<string>(
                name: "Clave",
                table: "gd$zonasalmacen",
                type: "varchar(200) CHARACTER SET utf8mb4",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Clave",
                table: "gd$posalmacen",
                type: "varchar(200) CHARACTER SET utf8mb4",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Clave",
                table: "gd$contalmacen",
                type: "varchar(200) CHARACTER SET utf8mb4",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Ubicacion",
                table: "gd$almacen",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_gd$zonasalmacen_Clave",
                table: "gd$zonasalmacen",
                column: "Clave");

            migrationBuilder.CreateIndex(
                name: "IX_gd$posalmacen_Clave",
                table: "gd$posalmacen",
                column: "Clave");

            migrationBuilder.CreateIndex(
                name: "IX_gd$contalmacen_Clave",
                table: "gd$contalmacen",
                column: "Clave");
        }
    }
}
