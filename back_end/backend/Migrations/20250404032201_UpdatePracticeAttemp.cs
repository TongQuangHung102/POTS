using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class UpdatePracticeAttemp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnswerPracticeQuestions_PracticeQuestions_QuestionId",
                table: "AnswerPracticeQuestions");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentAnswers_PracticeQuestions_QuestionId",
                table: "StudentAnswers");

            migrationBuilder.RenameColumn(
                name: "QuestionId",
                table: "PracticeQuestions",
                newName: "PracticeQuestionId");

            migrationBuilder.RenameColumn(
                name: "QuestionId",
                table: "AnswerPracticeQuestions",
                newName: "PracticeQuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_AnswerPracticeQuestions_QuestionId",
                table: "AnswerPracticeQuestions",
                newName: "IX_AnswerPracticeQuestions_PracticeQuestionId");

            migrationBuilder.AddColumn<int>(
                name: "PracticeQuestionId",
                table: "StudentAnswers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAIPractice",
                table: "PracticeAttempts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_StudentAnswers_PracticeQuestionId",
                table: "StudentAnswers",
                column: "PracticeQuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_AnswerPracticeQuestions_PracticeQuestions_PracticeQuestionId",
                table: "AnswerPracticeQuestions",
                column: "PracticeQuestionId",
                principalTable: "PracticeQuestions",
                principalColumn: "PracticeQuestionId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAnswers_PracticeQuestions_PracticeQuestionId",
                table: "StudentAnswers",
                column: "PracticeQuestionId",
                principalTable: "PracticeQuestions",
                principalColumn: "PracticeQuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAnswers_Questions_QuestionId",
                table: "StudentAnswers",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "QuestionId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnswerPracticeQuestions_PracticeQuestions_PracticeQuestionId",
                table: "AnswerPracticeQuestions");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentAnswers_PracticeQuestions_PracticeQuestionId",
                table: "StudentAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentAnswers_Questions_QuestionId",
                table: "StudentAnswers");

            migrationBuilder.DropIndex(
                name: "IX_StudentAnswers_PracticeQuestionId",
                table: "StudentAnswers");

            migrationBuilder.DropColumn(
                name: "PracticeQuestionId",
                table: "StudentAnswers");

            migrationBuilder.DropColumn(
                name: "IsAIPractice",
                table: "PracticeAttempts");

            migrationBuilder.RenameColumn(
                name: "PracticeQuestionId",
                table: "PracticeQuestions",
                newName: "QuestionId");

            migrationBuilder.RenameColumn(
                name: "PracticeQuestionId",
                table: "AnswerPracticeQuestions",
                newName: "QuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_AnswerPracticeQuestions_PracticeQuestionId",
                table: "AnswerPracticeQuestions",
                newName: "IX_AnswerPracticeQuestions_QuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_AnswerPracticeQuestions_PracticeQuestions_QuestionId",
                table: "AnswerPracticeQuestions",
                column: "QuestionId",
                principalTable: "PracticeQuestions",
                principalColumn: "QuestionId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAnswers_PracticeQuestions_QuestionId",
                table: "StudentAnswers",
                column: "QuestionId",
                principalTable: "PracticeQuestions",
                principalColumn: "QuestionId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
