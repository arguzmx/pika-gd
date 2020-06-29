using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Contacto.Data.Migrations
{
    public partial class InicialContaco : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "contacto$pais",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contacto$pais", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "contacto$tipofuentecontacto",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contacto$tipofuentecontacto", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "contacto$tipomediocontacto",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contacto$tipomediocontacto", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "contacto$estados",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false),
                    PaisId = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contacto$estados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_contacto$estados_contacto$pais_PaisId",
                        column: x => x.PaisId,
                        principalTable: "contacto$pais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "contacto$mediocontacto",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    TipoOrigenId = table.Column<string>(maxLength: 128, nullable: false),
                    OrigenId = table.Column<string>(maxLength: 128, nullable: false),
                    TipoMedioId = table.Column<string>(maxLength: 128, nullable: false),
                    TipoFuenteContactoId = table.Column<string>(maxLength: 128, nullable: true),
                    Principal = table.Column<bool>(nullable: false, defaultValue: false),
                    Activo = table.Column<bool>(nullable: false, defaultValue: true),
                    Medio = table.Column<string>(maxLength: 500, nullable: false),
                    Prefijo = table.Column<string>(maxLength: 100, nullable: true),
                    Sufijo = table.Column<string>(maxLength: 100, nullable: true),
                    Notas = table.Column<string>(maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contacto$mediocontacto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_contacto$mediocontacto_contacto$tipofuentecontacto_TipoFuent~",
                        column: x => x.TipoFuenteContactoId,
                        principalTable: "contacto$tipofuentecontacto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_contacto$mediocontacto_contacto$tipomediocontacto_TipoMedioId",
                        column: x => x.TipoMedioId,
                        principalTable: "contacto$tipomediocontacto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "contacto$direcciones",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false),
                    Calle = table.Column<string>(maxLength: 200, nullable: false),
                    NoInterno = table.Column<string>(maxLength: 200, nullable: true),
                    NoExterno = table.Column<string>(maxLength: 200, nullable: true),
                    Colonia = table.Column<string>(maxLength: 200, nullable: true),
                    CP = table.Column<string>(maxLength: 10, nullable: true),
                    Municipio = table.Column<string>(maxLength: 200, nullable: true),
                    EstadoId = table.Column<string>(maxLength: 128, nullable: true),
                    PaisId = table.Column<string>(maxLength: 128, nullable: true),
                    TipoOrigenId = table.Column<string>(maxLength: 128, nullable: false),
                    OrigenId = table.Column<string>(maxLength: 128, nullable: false),
                    EsDefault = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contacto$direcciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_contacto$direcciones_contacto$estados_EstadoId",
                        column: x => x.EstadoId,
                        principalTable: "contacto$estados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_contacto$direcciones_contacto$pais_PaisId",
                        column: x => x.PaisId,
                        principalTable: "contacto$pais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "contacto$horariomediocontacto",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    SinHorario = table.Column<bool>(nullable: false, defaultValue: false),
                    DiaSemana = table.Column<byte>(nullable: false, defaultValue: (byte)0),
                    Inicio = table.Column<DateTime>(nullable: true),
                    Fin = table.Column<DateTime>(nullable: true),
                    MedioContactoId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contacto$horariomediocontacto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_contacto$horariomediocontacto_contacto$mediocontacto_MedioCo~",
                        column: x => x.MedioContactoId,
                        principalTable: "contacto$mediocontacto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_contacto$direcciones_EstadoId",
                table: "contacto$direcciones",
                column: "EstadoId");

            migrationBuilder.CreateIndex(
                name: "IX_contacto$direcciones_PaisId",
                table: "contacto$direcciones",
                column: "PaisId");

            migrationBuilder.CreateIndex(
                name: "IX_contacto$direcciones_TipoOrigenId_OrigenId",
                table: "contacto$direcciones",
                columns: new[] { "TipoOrigenId", "OrigenId" });

            migrationBuilder.CreateIndex(
                name: "IX_contacto$estados_PaisId",
                table: "contacto$estados",
                column: "PaisId");

            migrationBuilder.CreateIndex(
                name: "IX_contacto$horariomediocontacto_MedioContactoId",
                table: "contacto$horariomediocontacto",
                column: "MedioContactoId");

            migrationBuilder.CreateIndex(
                name: "IX_contacto$mediocontacto_TipoFuenteContactoId",
                table: "contacto$mediocontacto",
                column: "TipoFuenteContactoId");

            migrationBuilder.CreateIndex(
                name: "IX_contacto$mediocontacto_TipoMedioId",
                table: "contacto$mediocontacto",
                column: "TipoMedioId");

            migrationBuilder.CreateIndex(
                name: "IX_contacto$mediocontacto_TipoOrigenId_OrigenId",
                table: "contacto$mediocontacto",
                columns: new[] { "TipoOrigenId", "OrigenId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "contacto$direcciones");

            migrationBuilder.DropTable(
                name: "contacto$horariomediocontacto");

            migrationBuilder.DropTable(
                name: "contacto$estados");

            migrationBuilder.DropTable(
                name: "contacto$mediocontacto");

            migrationBuilder.DropTable(
                name: "contacto$pais");

            migrationBuilder.DropTable(
                name: "contacto$tipofuentecontacto");

            migrationBuilder.DropTable(
                name: "contacto$tipomediocontacto");
        }
    }
}
