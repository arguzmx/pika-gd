using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Metadatos.Data.Migrations
{
    public partial class ActalizarMetadatos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_metadatos$almacendatos_metadatos$tipoalmacenmetadatos_TipoAl~",
                table: "metadatos$almacendatos");

            migrationBuilder.DropForeignKey(
                name: "FK_metadatos$asociacionplantilla_metadatos$plantilla_PlantillaId",
                table: "metadatos$asociacionplantilla");

            migrationBuilder.DropForeignKey(
                name: "FK_metadatos$propiedadplantilla_metadatos$plantilla_PlantillaId",
                table: "metadatos$propiedadplantilla");

            migrationBuilder.DropForeignKey(
                name: "FK_metadatos$propiedadplantilla_metadatos$tipodato_TipoDatoId",
                table: "metadatos$propiedadplantilla");

            migrationBuilder.DropForeignKey(
                name: "FK_metadatos$validadornumero_metadatos$propiedadplantilla_Propi~",
                table: "metadatos$validadornumero");

            migrationBuilder.DropForeignKey(
                name: "FK_metadatos$validadortexto_metadatos$propiedadplantilla_Propie~",
                table: "metadatos$validadortexto");

            migrationBuilder.DropColumn(
                name: "EsIdPadreJerarquia",
                table: "metadatos$propiedadplantilla");

            migrationBuilder.AddColumn<bool>(
                name: "UtilizarMax",
                table: "metadatos$validadornumero",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "UtilizarMin",
                table: "metadatos$validadornumero",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "AtributoListaPropiedadId",
                table: "metadatos$propiedadplantilla",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EsIdRaizJerarquia",
                table: "metadatos$propiedadplantilla",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Etiqueta",
                table: "metadatos$propiedadplantilla",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "OrdenarValoresListaPorNombre",
                table: "metadatos$propiedadplantilla",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "metadatos$atributoevento",
                columns: table => new
                {
                    PropiedadId = table.Column<string>(maxLength: 128, nullable: false),
                    Entidad = table.Column<string>(maxLength: 200, nullable: false),
                    Parametro = table.Column<string>(maxLength: 200, nullable: false),
                    Operacion = table.Column<int>(nullable: false),
                    Evento = table.Column<int>(nullable: false),
                    PropiedadPlantillaId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_metadatos$atributoevento", x => x.PropiedadId);
                    table.ForeignKey(
                        name: "FK_metadatos$atributoevento_metadatos$propiedadplantilla_Propie~",
                        column: x => x.PropiedadPlantillaId,
                        principalTable: "metadatos$propiedadplantilla",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "metadatos$atributolista",
                columns: table => new
                {
                    PropiedadId = table.Column<string>(maxLength: 128, nullable: false),
                    OrdenarAlfabetico = table.Column<bool>(nullable: false),
                    Entidad = table.Column<string>(maxLength: 200, nullable: false),
                    DatosRemotos = table.Column<bool>(nullable: false),
                    TypeAhead = table.Column<bool>(nullable: false),
                    Default = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_metadatos$atributolista", x => x.PropiedadId);
                });

            migrationBuilder.CreateTable(
                name: "metadatos$atributovistaUI",
                columns: table => new
                {
                    PropiedadId = table.Column<string>(maxLength: 128, nullable: false),
                    Control = table.Column<string>(maxLength: 200, nullable: false),
                    Accion = table.Column<int>(nullable: false),
                    Plataforma = table.Column<string>(maxLength: 200, nullable: false),
                    PropiedadPlantillaId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_metadatos$atributovistaUI", x => x.PropiedadId);
                    table.ForeignKey(
                        name: "FK_metadatos$atributovistaUI_metadatos$propiedadplantilla_Propi~",
                        column: x => x.PropiedadPlantillaId,
                        principalTable: "metadatos$propiedadplantilla",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "metadatos$catalogovinculado",
                columns: table => new
                {
                    IdCatalogo = table.Column<string>(maxLength: 128, nullable: false),
                    IdEntidad = table.Column<string>(maxLength: 128, nullable: false),
                    EntidadCatalogo = table.Column<string>(maxLength: 128, nullable: false),
                    IdCatalogoMap = table.Column<string>(maxLength: 128, nullable: false),
                    IdEntidadMap = table.Column<string>(maxLength: 128, nullable: false),
                    EntidadVinculo = table.Column<string>(maxLength: 128, nullable: false),
                    PropiedadReceptora = table.Column<string>(maxLength: 128, nullable: false),
                    Despliegue = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_metadatos$catalogovinculado", x => new { x.IdCatalogo, x.IdEntidad });
                });

            migrationBuilder.CreateTable(
                name: "metadatos$diccionarioentidadvinculada",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Enidad = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_metadatos$diccionarioentidadvinculada", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "metadatos$valorlista",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Texto = table.Column<string>(maxLength: 200, nullable: false),
                    Indice = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_metadatos$valorlista", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_metadatos$propiedadplantilla_AtributoListaPropiedadId",
                table: "metadatos$propiedadplantilla",
                column: "AtributoListaPropiedadId");

            migrationBuilder.CreateIndex(
                name: "IX_metadatos$atributoevento_PropiedadPlantillaId",
                table: "metadatos$atributoevento",
                column: "PropiedadPlantillaId");

            migrationBuilder.CreateIndex(
                name: "IX_metadatos$atributovistaUI_PropiedadPlantillaId",
                table: "metadatos$atributovistaUI",
                column: "PropiedadPlantillaId");

            migrationBuilder.AddForeignKey(
                name: "FK_metadatos$almacendatos_metadatos$tipoalmacenmetadatos_TipoAl~",
                table: "metadatos$almacendatos",
                column: "TipoAlmacenMetadatosId",
                principalTable: "metadatos$tipoalmacenmetadatos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_metadatos$asociacionplantilla_metadatos$plantilla_PlantillaId",
                table: "metadatos$asociacionplantilla",
                column: "PlantillaId",
                principalTable: "metadatos$plantilla",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_metadatos$propiedadplantilla_metadatos$atributolista_Atribut~",
                table: "metadatos$propiedadplantilla",
                column: "AtributoListaPropiedadId",
                principalTable: "metadatos$atributolista",
                principalColumn: "PropiedadId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_metadatos$propiedadplantilla_metadatos$plantilla_PlantillaId",
                table: "metadatos$propiedadplantilla",
                column: "PlantillaId",
                principalTable: "metadatos$plantilla",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_metadatos$propiedadplantilla_metadatos$tipodato_TipoDatoId",
                table: "metadatos$propiedadplantilla",
                column: "TipoDatoId",
                principalTable: "metadatos$tipodato",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_metadatos$almacendatos_metadatos$tipoalmacenmetadatos_TipoAl~",
                table: "metadatos$almacendatos");

            migrationBuilder.DropForeignKey(
                name: "FK_metadatos$asociacionplantilla_metadatos$plantilla_PlantillaId",
                table: "metadatos$asociacionplantilla");

            migrationBuilder.DropForeignKey(
                name: "FK_metadatos$propiedadplantilla_metadatos$atributolista_Atribut~",
                table: "metadatos$propiedadplantilla");

            migrationBuilder.DropForeignKey(
                name: "FK_metadatos$propiedadplantilla_metadatos$plantilla_PlantillaId",
                table: "metadatos$propiedadplantilla");

            migrationBuilder.DropForeignKey(
                name: "FK_metadatos$propiedadplantilla_metadatos$tipodato_TipoDatoId",
                table: "metadatos$propiedadplantilla");

            migrationBuilder.DropForeignKey(
                name: "FK_metadatos$validadornumero_metadatos$propiedadplantilla_Propi~",
                table: "metadatos$validadornumero");

            migrationBuilder.DropForeignKey(
                name: "FK_metadatos$validadortexto_metadatos$propiedadplantilla_Propie~",
                table: "metadatos$validadortexto");

            migrationBuilder.DropTable(
                name: "metadatos$atributoevento");

            migrationBuilder.DropTable(
                name: "metadatos$atributolista");

            migrationBuilder.DropTable(
                name: "metadatos$atributovistaUI");

            migrationBuilder.DropTable(
                name: "metadatos$catalogovinculado");

            migrationBuilder.DropTable(
                name: "metadatos$diccionarioentidadvinculada");

            migrationBuilder.DropTable(
                name: "metadatos$valorlista");

            migrationBuilder.DropIndex(
                name: "IX_metadatos$propiedadplantilla_AtributoListaPropiedadId",
                table: "metadatos$propiedadplantilla");

            migrationBuilder.DropColumn(
                name: "UtilizarMax",
                table: "metadatos$validadornumero");

            migrationBuilder.DropColumn(
                name: "UtilizarMin",
                table: "metadatos$validadornumero");

            migrationBuilder.DropColumn(
                name: "AtributoListaPropiedadId",
                table: "metadatos$propiedadplantilla");

            migrationBuilder.DropColumn(
                name: "EsIdRaizJerarquia",
                table: "metadatos$propiedadplantilla");

            migrationBuilder.DropColumn(
                name: "Etiqueta",
                table: "metadatos$propiedadplantilla");

            migrationBuilder.DropColumn(
                name: "OrdenarValoresListaPorNombre",
                table: "metadatos$propiedadplantilla");

            migrationBuilder.AddColumn<bool>(
                name: "EsIdPadreJerarquia",
                table: "metadatos$propiedadplantilla",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_metadatos$almacendatos_metadatos$tipoalmacenmetadatos_TipoAl~",
                table: "metadatos$almacendatos",
                column: "TipoAlmacenMetadatosId",
                principalTable: "metadatos$tipoalmacenmetadatos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_metadatos$asociacionplantilla_metadatos$plantilla_PlantillaId",
                table: "metadatos$asociacionplantilla",
                column: "PlantillaId",
                principalTable: "metadatos$plantilla",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_metadatos$propiedadplantilla_metadatos$plantilla_PlantillaId",
                table: "metadatos$propiedadplantilla",
                column: "PlantillaId",
                principalTable: "metadatos$plantilla",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_metadatos$propiedadplantilla_metadatos$tipodato_TipoDatoId",
                table: "metadatos$propiedadplantilla",
                column: "TipoDatoId",
                principalTable: "metadatos$tipodato",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
    }
}
