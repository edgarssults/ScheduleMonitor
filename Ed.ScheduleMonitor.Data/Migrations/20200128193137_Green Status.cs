using Microsoft.EntityFrameworkCore.Migrations;

namespace Ed.ScheduleMonitor.Data.Migrations
{
    public partial class GreenStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsGreen",
                table: "CalendarEvents",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsGreen",
                table: "CalendarEvents");
        }
    }
}
