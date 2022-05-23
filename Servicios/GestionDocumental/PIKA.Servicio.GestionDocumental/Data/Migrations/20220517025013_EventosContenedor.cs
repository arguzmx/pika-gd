using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.GestionDocumental.Data.Migrations
{
    public partial class EventosContenedor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FolioActualContenedor",
                table: "gd$almacen",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "HabilitarFoliado",
                table: "gd$almacen",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "MacroFolioContenedor",
                table: "gd$almacen",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Ubicacion",
                table: "gd$almacen",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "gd$zonasalmacen",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false),
                    Clave = table.Column<string>(maxLength: 200, nullable: false),
                    ArchivoId = table.Column<string>(maxLength: 128, nullable: false),
                    AlmacenArchivoId = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$zonasalmacen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_gd$zonasalmacen_gd$almacen_AlmacenArchivoId",
                        column: x => x.AlmacenArchivoId,
                        principalTable: "gd$almacen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_gd$zonasalmacen_gd$archivo_ArchivoId",
                        column: x => x.ArchivoId,
                        principalTable: "gd$archivo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "gd$posalmacen",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false),
                    Clave = table.Column<string>(maxLength: 200, nullable: false),
                    Indice = table.Column<int>(nullable: false),
                    CodigoBarras = table.Column<string>(maxLength: 512, nullable: true),
                    CodigoElectronico = table.Column<string>(maxLength: 512, nullable: true),
                    Ocupacion = table.Column<decimal>(nullable: false),
                    ArchivoId = table.Column<string>(maxLength: 128, nullable: false),
                    AlmacenArchivoId = table.Column<string>(maxLength: 128, nullable: false),
                    ZonaAlmacenId = table.Column<string>(maxLength: 128, nullable: false),
                    PosicionPadreId = table.Column<string>(maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$posalmacen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_gd$posalmacen_gd$almacen_AlmacenArchivoId",
                        column: x => x.AlmacenArchivoId,
                        principalTable: "gd$almacen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_gd$posalmacen_gd$archivo_ArchivoId",
                        column: x => x.ArchivoId,
                        principalTable: "gd$archivo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_gd$posalmacen_gd$posalmacen_PosicionPadreId",
                        column: x => x.PosicionPadreId,
                        principalTable: "gd$posalmacen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_gd$posalmacen_gd$zonasalmacen_ZonaAlmacenId",
                        column: x => x.ZonaAlmacenId,
                        principalTable: "gd$zonasalmacen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "gd$contalmacen",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false),
                    Clave = table.Column<string>(maxLength: 200, nullable: false),
                    CodigoBarras = table.Column<string>(maxLength: 512, nullable: true),
                    CodigoElectronico = table.Column<string>(maxLength: 512, nullable: true),
                    Ocupacion = table.Column<decimal>(nullable: false),
                    ArchivoId = table.Column<string>(maxLength: 128, nullable: false),
                    AlmacenArchivoId = table.Column<string>(maxLength: 128, nullable: false),
                    ZonaAlmacenId = table.Column<string>(maxLength: 128, nullable: true),
                    PosicionAlmacenId = table.Column<string>(maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$contalmacen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_gd$contalmacen_gd$almacen_AlmacenArchivoId",
                        column: x => x.AlmacenArchivoId,
                        principalTable: "gd$almacen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_gd$contalmacen_gd$archivo_ArchivoId",
                        column: x => x.ArchivoId,
                        principalTable: "gd$archivo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_gd$contalmacen_gd$posalmacen_PosicionAlmacenId",
                        column: x => x.PosicionAlmacenId,
                        principalTable: "gd$posalmacen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_gd$contalmacen_gd$zonasalmacen_ZonaAlmacenId",
                        column: x => x.ZonaAlmacenId,
                        principalTable: "gd$zonasalmacen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "gd$evtcontalmacen",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UsuarioId = table.Column<string>(maxLength: 128, nullable: true),
                    ProcesoId = table.Column<string>(maxLength: 128, nullable: true),
                    EsAccionUsuario = table.Column<bool>(nullable: false),
                    Fecha = table.Column<DateTime>(nullable: false),
                    TipoEvento = table.Column<int>(nullable: false),
                    ContenedorAlmacenId = table.Column<string>(maxLength: 128, nullable: false),
                    Payload = table.Column<string>(maxLength: 2048, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$evtcontalmacen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_gd$evtcontalmacen_gd$contalmacen_ContenedorAlmacenId",
                        column: x => x.ContenedorAlmacenId,
                        principalTable: "gd$contalmacen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_gd$almacen_Clave",
                table: "gd$almacen",
                column: "Clave");

            migrationBuilder.CreateIndex(
                name: "IX_gd$almacen_Nombre",
                table: "gd$almacen",
                column: "Nombre");

            migrationBuilder.CreateIndex(
                name: "IX_gd$contalmacen_AlmacenArchivoId",
                table: "gd$contalmacen",
                column: "AlmacenArchivoId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$contalmacen_ArchivoId",
                table: "gd$contalmacen",
                column: "ArchivoId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$contalmacen_Clave",
                table: "gd$contalmacen",
                column: "Clave");

            migrationBuilder.CreateIndex(
                name: "IX_gd$contalmacen_CodigoBarras",
                table: "gd$contalmacen",
                column: "CodigoBarras");

            migrationBuilder.CreateIndex(
                name: "IX_gd$contalmacen_CodigoElectronico",
                table: "gd$contalmacen",
                column: "CodigoElectronico");

            migrationBuilder.CreateIndex(
                name: "IX_gd$contalmacen_Nombre",
                table: "gd$contalmacen",
                column: "Nombre");

            migrationBuilder.CreateIndex(
                name: "IX_gd$contalmacen_PosicionAlmacenId",
                table: "gd$contalmacen",
                column: "PosicionAlmacenId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$contalmacen_ZonaAlmacenId",
                table: "gd$contalmacen",
                column: "ZonaAlmacenId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$evtcontalmacen_ContenedorAlmacenId",
                table: "gd$evtcontalmacen",
                column: "ContenedorAlmacenId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$posalmacen_AlmacenArchivoId",
                table: "gd$posalmacen",
                column: "AlmacenArchivoId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$posalmacen_ArchivoId",
                table: "gd$posalmacen",
                column: "ArchivoId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$posalmacen_Clave",
                table: "gd$posalmacen",
                column: "Clave");

            migrationBuilder.CreateIndex(
                name: "IX_gd$posalmacen_CodigoBarras",
                table: "gd$posalmacen",
                column: "CodigoBarras");

            migrationBuilder.CreateIndex(
                name: "IX_gd$posalmacen_CodigoElectronico",
                table: "gd$posalmacen",
                column: "CodigoElectronico");

            migrationBuilder.CreateIndex(
                name: "IX_gd$posalmacen_Nombre",
                table: "gd$posalmacen",
                column: "Nombre");

            migrationBuilder.CreateIndex(
                name: "IX_gd$posalmacen_PosicionPadreId",
                table: "gd$posalmacen",
                column: "PosicionPadreId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$posalmacen_ZonaAlmacenId",
                table: "gd$posalmacen",
                column: "ZonaAlmacenId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$zonasalmacen_AlmacenArchivoId",
                table: "gd$zonasalmacen",
                column: "AlmacenArchivoId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$zonasalmacen_ArchivoId",
                table: "gd$zonasalmacen",
                column: "ArchivoId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$zonasalmacen_Clave",
                table: "gd$zonasalmacen",
                column: "Clave");

            migrationBuilder.CreateIndex(
                name: "IX_gd$zonasalmacen_Nombre",
                table: "gd$zonasalmacen",
                column: "Nombre");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "gd$evtcontalmacen");

            migrationBuilder.DropTable(
                name: "gd$contalmacen");

            migrationBuilder.DropTable(
                name: "gd$posalmacen");

            migrationBuilder.DropTable(
                name: "gd$zonasalmacen");

            migrationBuilder.DropIndex(
                name: "IX_gd$almacen_Clave",
                table: "gd$almacen");

            migrationBuilder.DropIndex(
                name: "IX_gd$almacen_Nombre",
                table: "gd$almacen");

            migrationBuilder.DropColumn(
                name: "FolioActualContenedor",
                table: "gd$almacen");

            migrationBuilder.DropColumn(
                name: "HabilitarFoliado",
                table: "gd$almacen");

            migrationBuilder.DropColumn(
                name: "MacroFolioContenedor",
                table: "gd$almacen");

            migrationBuilder.DropColumn(
                name: "Ubicacion",
                table: "gd$almacen");
        }
    }
}
