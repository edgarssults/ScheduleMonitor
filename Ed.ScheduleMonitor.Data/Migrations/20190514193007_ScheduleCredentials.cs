using Microsoft.EntityFrameworkCore.Migrations;

namespace Ed.ScheduleMonitor.Data.Migrations
{
    public partial class ScheduleCredentials : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SchedulePassword",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ScheduleUsername",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SchedulePassword",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ScheduleUsername",
                table: "AspNetUsers");
        }
    }
}
