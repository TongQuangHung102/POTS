using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class AddTimeDouble : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "avg_Time",
                table: "StudentPerformances",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "TimePractice",
                table: "PracticeAttempts",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "avg_Time",
                table: "StudentPerformances");

            migrationBuilder.DropColumn(
                name: "TimePractice",
                table: "PracticeAttempts");
        }
    }
}
