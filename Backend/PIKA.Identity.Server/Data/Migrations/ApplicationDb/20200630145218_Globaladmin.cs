using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Identity.Server.Data.migrations.ApplicationDb
{
    public partial class Globaladmin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "GlobalAdmin",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GlobalAdmin",
                table: "AspNetUsers");
        }
    }
}
