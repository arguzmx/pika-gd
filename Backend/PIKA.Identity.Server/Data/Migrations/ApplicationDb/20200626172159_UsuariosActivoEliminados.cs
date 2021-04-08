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
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Inactiva",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropColumn(
                name: "Eliminada",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Inactiva",
                table: "AspNetUsers");


        }
    }
}
