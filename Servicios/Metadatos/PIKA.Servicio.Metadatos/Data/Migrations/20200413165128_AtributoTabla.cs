using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Metadatos.Data.Migrations
{
    public partial class AtributoTabla : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AtributoTablaid",
                table: "metadatos$propiedadplantilla",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipoDatoProiedadPlantillaId",
                table: "metadatos$propiedadplantilla",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "metadatos$atributotabla",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    PropiedadId = table.Column<string>(maxLength: 128, nullable: true),
                    Incluir = table.Column<bool>(nullable: false, defaultValue: true),
                    Visible = table.Column<bool>(nullable: false, defaultValue: true),
                    Alternable = table.Column<bool>(nullable: false, defaultValue: true),
                    IndiceOrdebnamiento = table.Column<int>(nullable: false, defaultValue: 1),
                    IdTablaCliente = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_metadatos$atributotabla", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_metadatos$propiedadplantilla_AtributoTablaid",
                table: "metadatos$propiedadplantilla",
                column: "AtributoTablaid",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_metadatos$propiedadplantilla_metadatos$atributotabla_Atribut~",
                table: "metadatos$propiedadplantilla",
                column: "AtributoTablaid",
                principalTable: "metadatos$atributotabla",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_metadatos$propiedadplantilla_metadatos$atributotabla_Atribut~",
                table: "metadatos$propiedadplantilla");

            migrationBuilder.DropTable(
                name: "metadatos$atributotabla");

            migrationBuilder.DropIndex(
                name: "IX_metadatos$propiedadplantilla_AtributoTablaid",
                table: "metadatos$propiedadplantilla");

            migrationBuilder.DropColumn(
                name: "AtributoTablaid",
                table: "metadatos$propiedadplantilla");

            migrationBuilder.DropColumn(
                name: "TipoDatoProiedadPlantillaId",
                table: "metadatos$propiedadplantilla");
        }
    }
}
