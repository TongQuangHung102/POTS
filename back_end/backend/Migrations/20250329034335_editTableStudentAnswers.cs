using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class editTableStudentAnswers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentAnswers_PracticeAttempts_AttemptId",
                table: "StudentAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentAnswers_PracticeAttempts_PracticeAttemptPracticeId",
                table: "StudentAnswers");

            migrationBuilder.DropIndex(
                name: "IX_StudentAnswers_PracticeAttemptPracticeId",
                table: "StudentAnswers");

            migrationBuilder.DropColumn(
                name: "PracticeAttemptPracticeId",
                table: "StudentAnswers");

            migrationBuilder.RenameColumn(
                name: "AttemptId",
                table: "StudentAnswers",
                newName: "PracticeId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentAnswers_AttemptId",
                table: "StudentAnswers",
                newName: "IX_StudentAnswers_PracticeId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAnswers_PracticeAttempts_PracticeId",
                table: "StudentAnswers",
                column: "PracticeId",
                principalTable: "PracticeAttempts",
                principalColumn: "PracticeId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentAnswers_PracticeAttempts_PracticeId",
                table: "StudentAnswers");

            migrationBuilder.RenameColumn(
                name: "PracticeId",
                table: "StudentAnswers",
                newName: "AttemptId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentAnswers_PracticeId",
                table: "StudentAnswers",
                newName: "IX_StudentAnswers_AttemptId");

            migrationBuilder.AddColumn<int>(
                name: "QuestionId1",
                table: "StudentAnswers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentAnswers_QuestionId1",
                table: "StudentAnswers",
                column: "QuestionId1");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAnswers_PracticeAttempts_AttemptId",
                table: "StudentAnswers",
                column: "AttemptId",
                principalTable: "PracticeAttempts",
                principalColumn: "PracticeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAnswers_Questions_QuestionId1",
                table: "StudentAnswers",
                column: "QuestionId1",
                principalTable: "Questions",
                principalColumn: "QuestionId");
        }
    }
}
