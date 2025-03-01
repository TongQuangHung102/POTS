using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class UpdatedRelationships : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuizAttempts_Users_UserId1",
                table: "QuizAttempts");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentPerformances_Users_UserId1",
                table: "StudentPerformances");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentProgresses_Users_UserId1",
                table: "StudentProgresses");

            migrationBuilder.DropIndex(
                name: "IX_StudentProgresses_UserId1",
                table: "StudentProgresses");

            migrationBuilder.DropIndex(
                name: "IX_StudentPerformances_UserId1",
                table: "StudentPerformances");

            migrationBuilder.DropIndex(
                name: "IX_QuizAttempts_UserId1",
                table: "QuizAttempts");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "StudentProgresses");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "StudentPerformances");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "QuizAttempts");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "StudentProgresses",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "StudentPerformances",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "QuizAttempts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentProgresses_UserId1",
                table: "StudentProgresses",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_StudentPerformances_UserId1",
                table: "StudentPerformances",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_QuizAttempts_UserId1",
                table: "QuizAttempts",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_QuizAttempts_Users_UserId1",
                table: "QuizAttempts",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentPerformances_Users_UserId1",
                table: "StudentPerformances",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentProgresses_Users_UserId1",
                table: "StudentProgresses",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "UserId");
        }
    }
}
