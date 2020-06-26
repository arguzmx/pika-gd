using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Seguridad.Data.Migrations
{
    public partial class PropiedadesUsuarios : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateTable(
            //    name: "aspnetuserclaims",
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
            //        table.PrimaryKey("PK_aspnetuserclaims", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_aspnetuserclaims_aspnetusers_UserId",
            //            column: x => x.UserId,
            //            principalTable: "aspnetusers",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            migrationBuilder.CreateTable(
                name: "seguridad$generousuario",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_seguridad$generousuario", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "seguridad$usuarioprops",
                columns: table => new
                {
                    UsuarioId = table.Column<string>(maxLength: 255, nullable: false),
                    username = table.Column<string>(maxLength: 200, nullable: false),
                    email = table.Column<string>(maxLength: 200, nullable: true),
                    name = table.Column<string>(maxLength: 200, nullable: true),
                    family_name = table.Column<string>(maxLength: 200, nullable: true),
                    given_name = table.Column<string>(maxLength: 200, nullable: true),
                    middle_name = table.Column<string>(maxLength: 200, nullable: true),
                    nickname = table.Column<string>(maxLength: 200, nullable: true),
                    updated_at = table.Column<DateTime>(nullable: true),
                    email_verified = table.Column<bool>(nullable: false, defaultValue: false),
                    generoid = table.Column<string>(maxLength: 128, nullable: true),
                    paisid = table.Column<string>(maxLength: 128, nullable: true),
                    estadoid = table.Column<string>(maxLength: 128, nullable: true),
                    gmt = table.Column<string>(maxLength: 255, nullable: true),
                    gmt_offset = table.Column<float>(nullable: true),
                    Ultimoacceso = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_seguridad$usuarioprops", x => x.UsuarioId);
                    table.ForeignKey(
                        name: "FK_seguridad$usuarioprops_aspnetusers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "aspnetusers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_seguridad$usuarioprops_seguridad$generousuario_generoid",
                        column: x => x.generoid,
                        principalTable: "seguridad$generousuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            //migrationBuilder.CreateIndex(
            //    name: "IX_aspnetuserclaims_UserId",
            //    table: "aspnetuserclaims",
            //    column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_seguridad$usuarioprops_generoid",
                table: "seguridad$usuarioprops",
                column: "generoid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "aspnetuserclaims");

            migrationBuilder.DropTable(
                name: "seguridad$usuarioprops");

            migrationBuilder.DropTable(
                name: "seguridad$generousuario");
        }
    }
}
