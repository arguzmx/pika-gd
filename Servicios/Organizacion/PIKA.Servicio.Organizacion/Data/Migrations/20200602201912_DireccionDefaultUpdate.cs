﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.Organizacion.Data.migrations
{
    public partial class DireccionDefaultUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Default",
                table: "org$direccion_postal");

            migrationBuilder.AddColumn<bool>(
                name: "EsDefault",
                table: "org$direccion_postal",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EsDefault",
                table: "org$direccion_postal");

            migrationBuilder.AddColumn<bool>(
                name: "Default",
                table: "org$direccion_postal",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }
    }
}
