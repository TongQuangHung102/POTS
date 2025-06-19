using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class DeleteLevelInPractice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PracticeAttempts_Levels_LevelId",
                table: "PracticeAttempts");

            migrationBuilder.DropIndex(
                name: "IX_PracticeAttempts_LevelId",
                table: "PracticeAttempts");

            migrationBuilder.DropColumn(
                name: "LevelId",
                table: "PracticeAttempts");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LevelId",
                table: "PracticeAttempts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PracticeAttempts_LevelId",
                table: "PracticeAttempts",
                column: "LevelId");

            migrationBuilder.AddForeignKey(
                name: "FK_PracticeAttempts_Levels_LevelId",
                table: "PracticeAttempts",
                column: "LevelId",
                principalTable: "Levels",
                principalColumn: "LevelId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
