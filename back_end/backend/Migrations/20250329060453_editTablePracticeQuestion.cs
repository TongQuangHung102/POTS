using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class editTablePracticeQuestion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PracticeQuestions_Levels_LevelId",
                table: "PracticeQuestions");

            migrationBuilder.DropIndex(
                name: "IX_PracticeQuestions_LevelId",
                table: "PracticeQuestions");

            migrationBuilder.DropColumn(
                name: "LevelId",
                table: "PracticeQuestions");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LevelId",
                table: "PracticeQuestions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PracticeQuestions_LevelId",
                table: "PracticeQuestions",
                column: "LevelId");

            migrationBuilder.AddForeignKey(
                name: "FK_PracticeQuestions_Levels_LevelId",
                table: "PracticeQuestions",
                column: "LevelId",
                principalTable: "Levels",
                principalColumn: "LevelId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
