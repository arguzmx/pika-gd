using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Seguridad.Data.Migrations
{
    public partial class UsuariosDominio : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            // ESTA TABÑLA ESTA CREADA EN LA MIGRACION DEL SERIVICIO DE SERVIDOR DE IDENTIDAD
            // -------------------------------------------------------------------------------
            // -------------------------------------------------------------------------------

            //migrationBuilder.CreateTable(
            //    name: "aspnetusers",
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
            //        AccessFailedCount = table.Column<int>(nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_aspnetusers", x => x.Id);
            //    });

            migrationBuilder.CreateTable(
                name: "seguridad$usuariosdominio",
                columns: table => new
                {
                    TipoOrigenId = table.Column<string>(maxLength: 128, nullable: false),
                    OrigenId = table.Column<string>(maxLength: 128, nullable: false),
                    ApplicationUserId = table.Column<string>(maxLength: 255, nullable: false),
                    EsAdmin = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_seguridad$usuariosdominio", x => new { x.ApplicationUserId, x.TipoOrigenId, x.OrigenId });
                    table.ForeignKey(
                        name: "FK_seguridad$usuariosdominio_aspnetusers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "aspnetusers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "seguridad$usuariosdominio");

            // ESTA TABÑLA ESTA CREADA EN LA MIGRACION DEL SERIVICIO DE SERVIDOR DE IDENTIDAD
            // -------------------------------------------------------------------------------
            // -------------------------------------------------------------------------------
            //migrationBuilder.DropTable(
            //    name: "aspnetusers");
        }
    }
}
