using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class UpdateDb284 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SampleQuestion",
                table: "PracticeAttempts");

            migrationBuilder.DropColumn(
                name: "LevelNumber",
                table: "Levels");

            migrationBuilder.DropColumn(
                name: "CreateByAI",
                table: "AIQuestions");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SampleQuestion",
                table: "PracticeAttempts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LevelNumber",
                table: "Levels",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "CreateByAI",
                table: "AIQuestions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
