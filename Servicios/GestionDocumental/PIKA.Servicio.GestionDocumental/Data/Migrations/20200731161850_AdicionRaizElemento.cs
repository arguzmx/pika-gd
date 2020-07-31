using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.GestionDocumental.data.Migrations
{
    public partial class AdicionRaizElemento : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_gd$elementoclasificacion_gd$cuadroclasificacion_CuadroClasif~",
                table: "gd$elementoclasificacion");

            migrationBuilder.DropForeignKey(
                name: "FK_gd$elementoclasificacion_gd$elementoclasificacion_ElementoCl~",
                table: "gd$elementoclasificacion");

            migrationBuilder.DropForeignKey(
                name: "FK_gd$tipoarchivo_gd$faseciclovital_FaseCicloVitalId",
                table: "gd$tipoarchivo");

            migrationBuilder.DropTable(
                name: "gd$faseciclovital");

            migrationBuilder.DropIndex(
                name: "IX_gd$tipoarchivo_FaseCicloVitalId",
                table: "gd$tipoarchivo");

            migrationBuilder.AddColumn<bool>(
                name: "EsRaiz",
                table: "gd$elementoclasificacion",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_gd$elementoclasificacion_gd$cuadroclasificacion_CuadroClasif~",
                table: "gd$elementoclasificacion",
                column: "CuadroClasifiacionId",
                principalTable: "gd$cuadroclasificacion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_gd$elementoclasificacion_gd$elementoclasificacion_ElementoCl~",
                table: "gd$elementoclasificacion",
                column: "ElementoClasificacionId",
                principalTable: "gd$elementoclasificacion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_gd$elementoclasificacion_gd$cuadroclasificacion_CuadroClasif~",
                table: "gd$elementoclasificacion");

            migrationBuilder.DropForeignKey(
                name: "FK_gd$elementoclasificacion_gd$elementoclasificacion_ElementoCl~",
                table: "gd$elementoclasificacion");

            migrationBuilder.DropColumn(
                name: "EsRaiz",
                table: "gd$elementoclasificacion");

            migrationBuilder.CreateTable(
                name: "gd$faseciclovital",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(type: "varchar(200) CHARACTER SET utf8mb4", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$faseciclovital", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_gd$tipoarchivo_FaseCicloVitalId",
                table: "gd$tipoarchivo",
                column: "FaseCicloVitalId");

            migrationBuilder.AddForeignKey(
                name: "FK_gd$elementoclasificacion_gd$cuadroclasificacion_CuadroClasif~",
                table: "gd$elementoclasificacion",
                column: "CuadroClasifiacionId",
                principalTable: "gd$cuadroclasificacion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_gd$elementoclasificacion_gd$elementoclasificacion_ElementoCl~",
                table: "gd$elementoclasificacion",
                column: "ElementoClasificacionId",
                principalTable: "gd$elementoclasificacion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_gd$tipoarchivo_gd$faseciclovital_FaseCicloVitalId",
                table: "gd$tipoarchivo",
                column: "FaseCicloVitalId",
                principalTable: "gd$faseciclovital",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
