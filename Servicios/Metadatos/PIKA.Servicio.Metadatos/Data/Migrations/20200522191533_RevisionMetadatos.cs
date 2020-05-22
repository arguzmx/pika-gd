using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Metadatos.Data.Migrations
{
    public partial class RevisionMetadatos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_metadatos$asociacionplantilla_metadatos$tipoalmacenmetadatos~",
                table: "metadatos$asociacionplantilla");

            migrationBuilder.DropForeignKey(
                name: "FK_metadatos$validadornumero_metadatos$propiedadplantilla_Propi~",
                table: "metadatos$validadornumero");

            migrationBuilder.DropForeignKey(
                name: "FK_metadatos$validadortexto_metadatos$propiedadplantilla_Propie~",
                table: "metadatos$validadortexto");

            migrationBuilder.DropIndex(
                name: "IX_metadatos$asociacionplantilla_TipoAlmacenMetadatosId",
                table: "metadatos$asociacionplantilla");

            migrationBuilder.DropColumn(
                name: "valordefaulr",
                table: "metadatos$validadortexto");

            migrationBuilder.DropColumn(
                name: "AsociacionPlantillaid",
                table: "metadatos$tipoalmacenmetadatos");

            migrationBuilder.DropColumn(
                name: "TipoAlmacenMetadatosId",
                table: "metadatos$asociacionplantilla");

            migrationBuilder.AlterColumn<string>(
                name: "regexp",
                table: "metadatos$validadortexto",
                maxLength: 1024,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "PropiedadId",
                table: "metadatos$validadortexto",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(128) CHARACTER SET utf8mb4",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "metadatos$validadortexto",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255) CHARACTER SET utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "valordefault",
                table: "metadatos$validadortexto",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PropiedadId",
                table: "metadatos$validadornumero",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(128) CHARACTER SET utf8mb4",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "metadatos$validadornumero",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255) CHARACTER SET utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "AlmacenDatosId",
                table: "metadatos$plantilla",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "metadatos$almacendatos",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: true),
                    Protocolo = table.Column<string>(maxLength: 50, nullable: true),
                    Direccion = table.Column<string>(maxLength: 50, nullable: false),
                    Usuario = table.Column<string>(maxLength: 50, nullable: true),
                    Contrasena = table.Column<string>(maxLength: 50, nullable: true),
                    Puerto = table.Column<string>(maxLength: 50, nullable: true),
                    TipoAlmacenMetadatosId = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_metadatos$almacendatos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_metadatos$almacendatos_metadatos$tipoalmacenmetadatos_TipoAl~",
                        column: x => x.TipoAlmacenMetadatosId,
                        principalTable: "metadatos$tipoalmacenmetadatos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_metadatos$plantilla_AlmacenDatosId",
                table: "metadatos$plantilla",
                column: "AlmacenDatosId");

            migrationBuilder.CreateIndex(
                name: "IX_metadatos$asociacionplantilla_TipoOrigenId_OrigenId",
                table: "metadatos$asociacionplantilla",
                columns: new[] { "TipoOrigenId", "OrigenId" });

            migrationBuilder.CreateIndex(
                name: "IX_metadatos$almacendatos_TipoAlmacenMetadatosId",
                table: "metadatos$almacendatos",
                column: "TipoAlmacenMetadatosId");

            migrationBuilder.AddForeignKey(
                name: "FK_metadatos$plantilla_metadatos$almacendatos_AlmacenDatosId",
                table: "metadatos$plantilla",
                column: "AlmacenDatosId",
                principalTable: "metadatos$almacendatos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_metadatos$validadornumero_metadatos$propiedadplantilla_Propi~",
                table: "metadatos$validadornumero",
                column: "PropiedadId",
                principalTable: "metadatos$propiedadplantilla",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_metadatos$validadortexto_metadatos$propiedadplantilla_Propie~",
                table: "metadatos$validadortexto",
                column: "PropiedadId",
                principalTable: "metadatos$propiedadplantilla",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_metadatos$plantilla_metadatos$almacendatos_AlmacenDatosId",
                table: "metadatos$plantilla");

            migrationBuilder.DropForeignKey(
                name: "FK_metadatos$validadornumero_metadatos$propiedadplantilla_Propi~",
                table: "metadatos$validadornumero");

            migrationBuilder.DropForeignKey(
                name: "FK_metadatos$validadortexto_metadatos$propiedadplantilla_Propie~",
                table: "metadatos$validadortexto");

            migrationBuilder.DropTable(
                name: "metadatos$almacendatos");

            migrationBuilder.DropIndex(
                name: "IX_metadatos$plantilla_AlmacenDatosId",
                table: "metadatos$plantilla");

            migrationBuilder.DropIndex(
                name: "IX_metadatos$asociacionplantilla_TipoOrigenId_OrigenId",
                table: "metadatos$asociacionplantilla");

            migrationBuilder.DropColumn(
                name: "valordefault",
                table: "metadatos$validadortexto");

            migrationBuilder.DropColumn(
                name: "AlmacenDatosId",
                table: "metadatos$plantilla");

            migrationBuilder.AlterColumn<string>(
                name: "regexp",
                table: "metadatos$validadortexto",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 1024,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PropiedadId",
                table: "metadatos$validadortexto",
                type: "varchar(128) CHARACTER SET utf8mb4",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "metadatos$validadortexto",
                type: "varchar(255) CHARACTER SET utf8mb4",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 128);

            migrationBuilder.AddColumn<string>(
                name: "valordefaulr",
                table: "metadatos$validadortexto",
                type: "varchar(128) CHARACTER SET utf8mb4",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "PropiedadId",
                table: "metadatos$validadornumero",
                type: "varchar(128) CHARACTER SET utf8mb4",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "metadatos$validadornumero",
                type: "varchar(255) CHARACTER SET utf8mb4",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 128);

            migrationBuilder.AddColumn<string>(
                name: "AsociacionPlantillaid",
                table: "metadatos$tipoalmacenmetadatos",
                type: "varchar(128) CHARACTER SET utf8mb4",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TipoAlmacenMetadatosId",
                table: "metadatos$asociacionplantilla",
                type: "varchar(128) CHARACTER SET utf8mb4",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_metadatos$asociacionplantilla_TipoAlmacenMetadatosId",
                table: "metadatos$asociacionplantilla",
                column: "TipoAlmacenMetadatosId");

            migrationBuilder.AddForeignKey(
                name: "FK_metadatos$asociacionplantilla_metadatos$tipoalmacenmetadatos~",
                table: "metadatos$asociacionplantilla",
                column: "TipoAlmacenMetadatosId",
                principalTable: "metadatos$tipoalmacenmetadatos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_metadatos$validadornumero_metadatos$propiedadplantilla_Propi~",
                table: "metadatos$validadornumero",
                column: "PropiedadId",
                principalTable: "metadatos$propiedadplantilla",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_metadatos$validadortexto_metadatos$propiedadplantilla_Propie~",
                table: "metadatos$validadortexto",
                column: "PropiedadId",
                principalTable: "metadatos$propiedadplantilla",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
