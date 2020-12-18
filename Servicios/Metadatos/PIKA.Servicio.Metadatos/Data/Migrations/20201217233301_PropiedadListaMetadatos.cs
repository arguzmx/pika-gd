using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Metadatos.data.Migrations
{
    public partial class PropiedadListaMetadatos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_metadatos$propiedadplantilla_metadatos$atributolista_Atribut~",
                table: "metadatos$propiedadplantilla");

            migrationBuilder.DropForeignKey(
                name: "FK_metadatos$valorespropiedad_metadatos$propiedadplantilla_Prop~",
                table: "metadatos$valorespropiedad");

            migrationBuilder.DropTable(
                name: "metadatos$asociacionplantilla");

            migrationBuilder.DropTable(
                name: "metadatos$atributoevento");

            migrationBuilder.DropTable(
                name: "metadatos$atributolista");

            migrationBuilder.DropTable(
                name: "metadatos$atributotabla");

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

            migrationBuilder.DropPrimaryKey(
                name: "PK_metadatos$valorespropiedad",
                table: "metadatos$valorespropiedad");

            migrationBuilder.DropColumn(
                name: "AtributoListaPropiedadId",
                table: "metadatos$propiedadplantilla");

            migrationBuilder.RenameTable(
                name: "metadatos$valorespropiedad",
                newName: "metadatos$valoreslistapropiedad");

            migrationBuilder.RenameIndex(
                name: "IX_metadatos$valorespropiedad_PropiedadId",
                table: "metadatos$valoreslistapropiedad",
                newName: "IX_metadatos$valoreslistapropiedad_PropiedadId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_metadatos$valoreslistapropiedad",
                table: "metadatos$valoreslistapropiedad",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_metadatos$valoreslistapropiedad_metadatos$propiedadplantilla~",
                table: "metadatos$valoreslistapropiedad",
                column: "PropiedadId",
                principalTable: "metadatos$propiedadplantilla",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_metadatos$valoreslistapropiedad_metadatos$propiedadplantilla~",
                table: "metadatos$valoreslistapropiedad");

            migrationBuilder.DropPrimaryKey(
                name: "PK_metadatos$valoreslistapropiedad",
                table: "metadatos$valoreslistapropiedad");

            migrationBuilder.RenameTable(
                name: "metadatos$valoreslistapropiedad",
                newName: "metadatos$valorespropiedad");

            migrationBuilder.RenameIndex(
                name: "IX_metadatos$valoreslistapropiedad_PropiedadId",
                table: "metadatos$valorespropiedad",
                newName: "IX_metadatos$valorespropiedad_PropiedadId");

            migrationBuilder.AddColumn<string>(
                name: "AtributoListaPropiedadId",
                table: "metadatos$propiedadplantilla",
                type: "varchar(128) CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_metadatos$valorespropiedad",
                table: "metadatos$valorespropiedad",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "metadatos$asociacionplantilla",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false),
                    IdentificadorAlmacenamiento = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false),
                    OrigenId = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false),
                    PlantillaId = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false),
                    TipoOrigenId = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_metadatos$asociacionplantilla", x => x.Id);
                    table.ForeignKey(
                        name: "FK_metadatos$asociacionplantilla_metadatos$plantilla_PlantillaId",
                        column: x => x.PlantillaId,
                        principalTable: "metadatos$plantilla",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "metadatos$atributoevento",
                columns: table => new
                {
                    PropiedadId = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false),
                    Entidad = table.Column<string>(type: "varchar(200) CHARACTER SET utf8mb4", maxLength: 200, nullable: false),
                    Evento = table.Column<int>(type: "int", nullable: false),
                    Operacion = table.Column<int>(type: "int", nullable: false),
                    Parametro = table.Column<string>(type: "varchar(200) CHARACTER SET utf8mb4", maxLength: 200, nullable: false),
                    PropiedadPlantillaId = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", nullable: true)
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
                    PropiedadId = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false),
                    DatosRemotos = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Default = table.Column<string>(type: "varchar(200) CHARACTER SET utf8mb4", maxLength: 200, nullable: false),
                    Entidad = table.Column<string>(type: "varchar(200) CHARACTER SET utf8mb4", maxLength: 200, nullable: false),
                    OrdenarAlfabetico = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TypeAhead = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_metadatos$atributolista", x => x.PropiedadId);
                });

            migrationBuilder.CreateTable(
                name: "metadatos$atributotabla",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255) CHARACTER SET utf8mb4", nullable: false),
                    Alternable = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    IdTablaCliente = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false),
                    Incluir = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    IndiceOrdebnamiento = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    PropiedadId = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: true),
                    Visible = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_metadatos$atributotabla", x => x.Id);
                    table.ForeignKey(
                        name: "FK_metadatos$atributotabla_metadatos$propiedadplantilla_Propied~",
                        column: x => x.PropiedadId,
                        principalTable: "metadatos$propiedadplantilla",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "metadatos$atributovistaUI",
                columns: table => new
                {
                    PropiedadId = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false),
                    Accion = table.Column<int>(type: "int", nullable: false),
                    Control = table.Column<string>(type: "varchar(200) CHARACTER SET utf8mb4", maxLength: 200, nullable: false),
                    Plataforma = table.Column<string>(type: "varchar(200) CHARACTER SET utf8mb4", maxLength: 200, nullable: false),
                    PropiedadPlantillaId = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", nullable: true)
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
                    IdCatalogo = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false),
                    IdEntidad = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false),
                    Despliegue = table.Column<int>(type: "int", nullable: false),
                    EntidadCatalogo = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false),
                    EntidadVinculo = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false),
                    IdCatalogoMap = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false),
                    IdEntidadMap = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false),
                    PropiedadReceptora = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_metadatos$catalogovinculado", x => new { x.IdCatalogo, x.IdEntidad });
                });

            migrationBuilder.CreateTable(
                name: "metadatos$diccionarioentidadvinculada",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false),
                    Enidad = table.Column<string>(type: "varchar(200) CHARACTER SET utf8mb4", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_metadatos$diccionarioentidadvinculada", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "metadatos$valorlista",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false),
                    Indice = table.Column<int>(type: "int", nullable: false),
                    Texto = table.Column<string>(type: "varchar(200) CHARACTER SET utf8mb4", maxLength: 200, nullable: false)
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
                name: "IX_metadatos$asociacionplantilla_PlantillaId",
                table: "metadatos$asociacionplantilla",
                column: "PlantillaId");

            migrationBuilder.CreateIndex(
                name: "IX_metadatos$asociacionplantilla_TipoOrigenId_OrigenId",
                table: "metadatos$asociacionplantilla",
                columns: new[] { "TipoOrigenId", "OrigenId" });

            migrationBuilder.CreateIndex(
                name: "IX_metadatos$atributoevento_PropiedadPlantillaId",
                table: "metadatos$atributoevento",
                column: "PropiedadPlantillaId");

            migrationBuilder.CreateIndex(
                name: "IX_metadatos$atributotabla_PropiedadId",
                table: "metadatos$atributotabla",
                column: "PropiedadId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_metadatos$atributovistaUI_PropiedadPlantillaId",
                table: "metadatos$atributovistaUI",
                column: "PropiedadPlantillaId");

            migrationBuilder.AddForeignKey(
                name: "FK_metadatos$propiedadplantilla_metadatos$atributolista_Atribut~",
                table: "metadatos$propiedadplantilla",
                column: "AtributoListaPropiedadId",
                principalTable: "metadatos$atributolista",
                principalColumn: "PropiedadId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_metadatos$valorespropiedad_metadatos$propiedadplantilla_Prop~",
                table: "metadatos$valorespropiedad",
                column: "PropiedadId",
                principalTable: "metadatos$propiedadplantilla",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
