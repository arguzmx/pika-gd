using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Metadatos.Data.Migrations
{
    public partial class InicialMetadatos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "metadatos$tipoalmacenmetadatos",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_metadatos$tipoalmacenmetadatos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "metadatos$tipodato",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_metadatos$tipodato", x => x.Id);
                });

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
                        name: "FK_m$almacendatos_m$tipoalmacenm_TipoAl",
                        column: x => x.TipoAlmacenMetadatosId,
                        principalTable: "metadatos$tipoalmacenmetadatos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "metadatos$plantilla",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false),
                    TipoOrigenId = table.Column<string>(maxLength: 128, nullable: false),
                    OrigenId = table.Column<string>(maxLength: 128, nullable: false),
                    Eliminada = table.Column<bool>(nullable: false),
                    Generada = table.Column<bool>(nullable: false),
                    AlmacenDatosId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_metadatos$plantilla", x => x.Id);
                    table.ForeignKey(
                        name: "FK_m$plantilla_m$almacendatos_AlmacenDatosId",
                        column: x => x.AlmacenDatosId,
                        principalTable: "metadatos$almacendatos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "metadatos$propiedadplantilla",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false),
                    TipoDatoId = table.Column<string>(maxLength: 128, nullable: false),
                    ValorDefault = table.Column<string>(type: "TEXT", nullable: true),
                    Requerido = table.Column<bool>(nullable: false),
                    EsIndice = table.Column<bool>(nullable: false),
                    Buscable = table.Column<bool>(nullable: false),
                    Ordenable = table.Column<bool>(nullable: false),
                    Visible = table.Column<bool>(nullable: false),
                    IndiceOrdenamiento = table.Column<int>(nullable: false),
                    EsIdClaveExterna = table.Column<bool>(nullable: false),
                    EsIdRegistro = table.Column<bool>(nullable: false),
                    EsIdJerarquia = table.Column<bool>(nullable: false),
                    EsTextoJerarquia = table.Column<bool>(nullable: false),
                    EsIdRaizJerarquia = table.Column<bool>(nullable: false),
                    EsFiltroJerarquia = table.Column<bool>(nullable: false),
                    Autogenerado = table.Column<bool>(nullable: false),
                    OrdenarValoresListaPorNombre = table.Column<bool>(nullable: false),
                    ControlHTML = table.Column<string>(maxLength: 128, nullable: false),
                    Etiqueta = table.Column<bool>(nullable: false),
                    IdNumericoPlantilla = table.Column<int>(nullable: false),
                    PlantillaId = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_metadatos$propiedadplantilla", x => x.Id);
                    table.ForeignKey(
                        name: "FK_m$propiedadpl_m$plantilla_PlantillaId",
                        column: x => x.PlantillaId,
                        principalTable: "metadatos$plantilla",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_m$propiedadpl_me$tipodato_TipoDatoId",
                        column: x => x.TipoDatoId,
                        principalTable: "metadatos$tipodato",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "metadatos$validadornumero",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    PropiedadId = table.Column<string>(maxLength: 128, nullable: false),
                    min = table.Column<float>(nullable: false),
                    max = table.Column<float>(nullable: false),
                    UtilizarMax = table.Column<bool>(nullable: false),
                    UtilizarMin = table.Column<bool>(nullable: false),
                    valordefault = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_metadatos$validadornumero", x => x.Id);
                    table.ForeignKey(
                        name: "FK_m$validadornumero_m$propiedadpl_Propi",
                        column: x => x.PropiedadId,
                        principalTable: "metadatos$propiedadplantilla",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "metadatos$validadortexto",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    PropiedadId = table.Column<string>(maxLength: 128, nullable: false),
                    longmin = table.Column<int>(nullable: false),
                    longmax = table.Column<int>(nullable: false),
                    regexp = table.Column<string>(maxLength: 1024, nullable: true),
                    valordefault = table.Column<string>(maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_metadatos$validadortexto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_m$validadortexto_m$propiedadpl_Propi",
                        column: x => x.PropiedadId,
                        principalTable: "metadatos$propiedadplantilla",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "metadatos$valoreslistapropiedad",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Texto = table.Column<string>(maxLength: 200, nullable: false),
                    Indice = table.Column<int>(nullable: false, defaultValue: 0),
                    PropiedadId = table.Column<string>(maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_metadatos$valoreslistapropiedad", x => x.Id);
                    table.ForeignKey(
                        name: "FK_m$valoreslp_me$proppl",
                        column: x => x.PropiedadId,
                        principalTable: "metadatos$propiedadplantilla",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_metadatos$almacendatos_TipoAlmacenMetadatosId",
                table: "metadatos$almacendatos",
                column: "TipoAlmacenMetadatosId");

            migrationBuilder.CreateIndex(
                name: "IX_m$plantilla_AlmacenDatosId",
                table: "metadatos$plantilla",
                column: "AlmacenDatosId");

            migrationBuilder.CreateIndex(
                name: "IX_m$propiedadplantilla_PlantillaId",
                table: "metadatos$propiedadplantilla",
                column: "PlantillaId");

            migrationBuilder.CreateIndex(
                name: "IX_m$propiedadplantilla_TipoDatoId",
                table: "metadatos$propiedadplantilla",
                column: "TipoDatoId");

            migrationBuilder.CreateIndex(
                name: "IX_m$validadornumero_PropiedadId",
                table: "metadatos$validadornumero",
                column: "PropiedadId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_m$validadortexto_PropiedadId",
                table: "metadatos$validadortexto",
                column: "PropiedadId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_m$valspropiedad_PropId",
                table: "metadatos$valoreslistapropiedad",
                column: "PropiedadId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "metadatos$validadornumero");

            migrationBuilder.DropTable(
                name: "metadatos$validadortexto");

            migrationBuilder.DropTable(
                name: "metadatos$valoreslistapropiedad");

            migrationBuilder.DropTable(
                name: "metadatos$propiedadplantilla");

            migrationBuilder.DropTable(
                name: "metadatos$plantilla");

            migrationBuilder.DropTable(
                name: "metadatos$tipodato");

            migrationBuilder.DropTable(
                name: "metadatos$almacendatos");

            migrationBuilder.DropTable(
                name: "metadatos$tipoalmacenmetadatos");
        }
    }
}
