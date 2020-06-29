using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Identity.Server.Data.Migrations.ApplicationDb
{
    public partial class UsuariosActivoEliminados : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<bool>(
                name: "Eliminada",
                table: "aspnetusers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Inactiva",
                table: "aspnetusers",
                nullable: false,
                defaultValue: false);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropColumn(
                name: "Eliminada",
                table: "aspnetusers");

            migrationBuilder.DropColumn(
                name: "Inactiva",
                table: "aspnetusers");


        }
    }
}
