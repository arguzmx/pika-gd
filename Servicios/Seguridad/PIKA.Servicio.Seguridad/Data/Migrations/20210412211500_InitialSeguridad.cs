using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Seguridad.Data.Migrations
{
    public partial class InitialSeguridad : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateTable(
            //    name: "AspNetUsers",
            //    columns: table => new
            //    {
            //        Id = table.Column<string>(maxLength: 255, nullable: false),
            //        LockoutEnd = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
            //        TwoFactorEnabled = table.Column<bool>(nullable: false),
            //        PhoneNumberConfirmed = table.Column<bool>(nullable: false),
            //        PhoneNumber = table.Column<string>(type: "longtext", nullable: true),
            //        ConcurrencyStamp = table.Column<string>(nullable: true),
            //        SecurityStamp = table.Column<string>(type: "longtext", nullable: true),
            //        PasswordHash = table.Column<string>(type: "longtext", nullable: true),
            //        EmailConfirmed = table.Column<bool>(nullable: false),
            //        NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
            //        Email = table.Column<string>(maxLength: 256, nullable: true),
            //        NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
            //        UserName = table.Column<string>(maxLength: 256, nullable: true),
            //        LockoutEnabled = table.Column<bool>(nullable: false),
            //        AccessFailedCount = table.Column<int>(nullable: false),
            //        Inactiva = table.Column<bool>(nullable: false, defaultValue: false),
            //        Eliminada = table.Column<bool>(nullable: false, defaultValue: false),
            //        GlobalAdmin = table.Column<bool>(nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_AspNetUsers", x => x.Id);
            //    });

            migrationBuilder.CreateTable(
                name: "seguridad$aplicacion",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(maxLength: 500, nullable: false),
                    UICulture = table.Column<string>(maxLength: 10, nullable: false),
                    Version = table.Column<string>(maxLength: 10, nullable: false),
                    ReleaseIndex = table.Column<int>(nullable: false, defaultValue: 10)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_seguridad$aplicacion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "seguridad$generousuario",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_seguridad$generousuario", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "seguridad$permisosapl",
                columns: table => new
                {
                    DominioId = table.Column<string>(maxLength: 128, nullable: false),
                    AplicacionId = table.Column<string>(maxLength: 128, nullable: false),
                    ModuloId = table.Column<string>(maxLength: 128, nullable: false),
                    TipoEntidadAcceso = table.Column<string>(maxLength: 128, nullable: false),
                    EntidadAccesoId = table.Column<string>(maxLength: 128, nullable: false),
                    Leer = table.Column<bool>(nullable: false),
                    NegarAcceso = table.Column<bool>(nullable: false),
                    Escribir = table.Column<bool>(nullable: false),
                    Eliminar = table.Column<bool>(nullable: false),
                    Admin = table.Column<bool>(nullable: false),
                    Ejecutar = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_seguridad$permisosapl", x => new { x.DominioId, x.AplicacionId, x.ModuloId, x.EntidadAccesoId, x.TipoEntidadAcceso });
                });

            //migrationBuilder.CreateTable(
            //    name: "AspNetUserClaims",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(nullable: false)
            //            .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
            //        UserId = table.Column<string>(maxLength: 255, nullable: false),
            //        ClaimType = table.Column<string>(type: "longtext", nullable: true),
            //        ClaimValue = table.Column<string>(type: "longtext", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_AspNetUserClaims_AspNetUsers_UserId",
            //            column: x => x.UserId,
            //            principalTable: "AspNetUsers",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            migrationBuilder.CreateTable(
                name: "seguridad$usuariosdominio",
                columns: table => new
                {
                    UnidadOrganizacionalId = table.Column<string>(maxLength: 128, nullable: false),
                    DominioId = table.Column<string>(maxLength: 128, nullable: false),
                    ApplicationUserId = table.Column<string>(maxLength: 255, nullable: false),
                    EsAdmin = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_seguridad$usuariosdominio", x => new { x.ApplicationUserId, x.DominioId, x.UnidadOrganizacionalId });
                    table.ForeignKey(
                        name: "FK_seguridad$usuariosdominio_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "seguridad$moduloaplicacion",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Tipo = table.Column<int>(nullable: false),
                    AplicacionId = table.Column<string>(maxLength: 128, nullable: false),
                    Asegurable = table.Column<bool>(nullable: false, defaultValue: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(maxLength: 500, nullable: false),
                    ModuloPadreId = table.Column<string>(maxLength: 128, nullable: true),
                    AplicacionPadreId = table.Column<string>(nullable: true),
                    Icono = table.Column<string>(maxLength: 100, nullable: false),
                    UICulture = table.Column<string>(maxLength: 10, nullable: false),
                    PermisosDisponibles = table.Column<ulong>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_seguridad$moduloaplicacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_seguridad$moduloaplicacion_seguridad$aplicacion_AplicacionId",
                        column: x => x.AplicacionId,
                        principalTable: "seguridad$aplicacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_seguridad$moduloaplicacion_seguridad$moduloaplicacion_Modulo~",
                        column: x => x.ModuloPadreId,
                        principalTable: "seguridad$moduloaplicacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "seguridad$usuarioprops",
                columns: table => new
                {
                    UsuarioId = table.Column<string>(maxLength: 255, nullable: false),
                    username = table.Column<string>(maxLength: 200, nullable: false),
                    email = table.Column<string>(maxLength: 200, nullable: true),
                    Inactiva = table.Column<bool>(nullable: false),
                    name = table.Column<string>(maxLength: 200, nullable: true),
                    family_name = table.Column<string>(maxLength: 200, nullable: true),
                    given_name = table.Column<string>(maxLength: 200, nullable: true),
                    middle_name = table.Column<string>(maxLength: 200, nullable: true),
                    nickname = table.Column<string>(maxLength: 200, nullable: true),
                    generoid = table.Column<string>(maxLength: 128, nullable: true),
                    paisid = table.Column<string>(maxLength: 128, nullable: true),
                    estadoid = table.Column<string>(maxLength: 128, nullable: true),
                    Eliminada = table.Column<bool>(nullable: false),
                    gmt = table.Column<string>(maxLength: 255, nullable: true),
                    gmt_offset = table.Column<float>(nullable: true),
                    updated_at = table.Column<DateTime>(nullable: true),
                    email_verified = table.Column<bool>(nullable: false, defaultValue: false),
                    Ultimoacceso = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_seguridad$usuarioprops", x => x.UsuarioId);
                    table.ForeignKey(
                        name: "FK_seguridad$usuarioprops_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_seguridad$usuarioprops_seguridad$generousuario_generoid",
                        column: x => x.generoid,
                        principalTable: "seguridad$generousuario",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "seguridad$tipoadministradormodulo",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    AplicacionId = table.Column<string>(maxLength: 128, nullable: false),
                    ModuloId = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_seguridad$tipoadministradormodulo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_seguridad$tipoadministradormodulo_seguridad$moduloaplicacion~",
                        column: x => x.ModuloId,
                        principalTable: "seguridad$moduloaplicacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "seguridad$traduccionaplicacionmodulo",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    AplicacionId = table.Column<string>(maxLength: 128, nullable: false),
                    ModuloId = table.Column<string>(maxLength: 128, nullable: false),
                    UICulture = table.Column<string>(maxLength: 10, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_seguridad$traduccionaplicacionmodulo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_seguridad$traduccionaplicacionmodulo_seguridad$aplicacion_Ap~",
                        column: x => x.AplicacionId,
                        principalTable: "seguridad$aplicacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_seguridad$traduccionaplicacionmodulo_seguridad$moduloaplicac~",
                        column: x => x.ModuloId,
                        principalTable: "seguridad$moduloaplicacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            //migrationBuilder.CreateIndex(
            //    name: "IX_AspNetUserClaims_UserId",
            //    table: "AspNetUserClaims",
            //    column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_seguridad$moduloaplicacion_AplicacionId",
                table: "seguridad$moduloaplicacion",
                column: "AplicacionId");

            migrationBuilder.CreateIndex(
                name: "IX_seguridad$moduloaplicacion_ModuloPadreId",
                table: "seguridad$moduloaplicacion",
                column: "ModuloPadreId");

            migrationBuilder.CreateIndex(
                name: "IX_seguridad$tipoadministradormodulo_ModuloId",
                table: "seguridad$tipoadministradormodulo",
                column: "ModuloId");

            migrationBuilder.CreateIndex(
                name: "IX_seguridad$traduccionaplicacionmodulo_AplicacionId",
                table: "seguridad$traduccionaplicacionmodulo",
                column: "AplicacionId");

            migrationBuilder.CreateIndex(
                name: "IX_seguridad$traduccionaplicacionmodulo_ModuloId",
                table: "seguridad$traduccionaplicacionmodulo",
                column: "ModuloId");

            migrationBuilder.CreateIndex(
                name: "IX_seguridad$usuarioprops_generoid",
                table: "seguridad$usuarioprops",
                column: "generoid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "seguridad$permisosapl");

            migrationBuilder.DropTable(
                name: "seguridad$tipoadministradormodulo");

            migrationBuilder.DropTable(
                name: "seguridad$traduccionaplicacionmodulo");

            migrationBuilder.DropTable(
                name: "seguridad$usuarioprops");

            migrationBuilder.DropTable(
                name: "seguridad$usuariosdominio");

            migrationBuilder.DropTable(
                name: "seguridad$moduloaplicacion");

            migrationBuilder.DropTable(
                name: "seguridad$generousuario");

            //migrationBuilder.DropTable(
            //    name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "seguridad$aplicacion");
        }
    }
}
