using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class FixTestQuestions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestQuestion_Questions_QuestionId",
                table: "TestQuestion");

            migrationBuilder.DropForeignKey(
                name: "FK_TestQuestion_Tests_TestId",
                table: "TestQuestion");

            migrationBuilder.DropForeignKey(
                name: "FK_TestSubmission_Questions_QuestionId",
                table: "TestSubmission");

            migrationBuilder.DropForeignKey(
                name: "FK_TestSubmission_StudentTest_StudentTestId",
                table: "TestSubmission");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TestSubmission",
                table: "TestSubmission");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TestQuestion",
                table: "TestQuestion");

            migrationBuilder.RenameTable(
                name: "TestSubmission",
                newName: "TestSubmissions");

            migrationBuilder.RenameTable(
                name: "TestQuestion",
                newName: "TestQuestions");

            migrationBuilder.RenameIndex(
                name: "IX_TestSubmission_StudentTestId",
                table: "TestSubmissions",
                newName: "IX_TestSubmissions_StudentTestId");

            migrationBuilder.RenameIndex(
                name: "IX_TestSubmission_QuestionId",
                table: "TestSubmissions",
                newName: "IX_TestSubmissions_QuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_TestQuestion_TestId",
                table: "TestQuestions",
                newName: "IX_TestQuestions_TestId");

            migrationBuilder.RenameIndex(
                name: "IX_TestQuestion_QuestionId",
                table: "TestQuestions",
                newName: "IX_TestQuestions_QuestionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TestSubmissions",
                table: "TestSubmissions",
                column: "SubmissionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TestQuestions",
                table: "TestQuestions",
                column: "TestQuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestQuestions_Questions_QuestionId",
                table: "TestQuestions",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "QuestionId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestQuestions_Tests_TestId",
                table: "TestQuestions",
                column: "TestId",
                principalTable: "Tests",
                principalColumn: "TestId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestSubmissions_Questions_QuestionId",
                table: "TestSubmissions",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "QuestionId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestSubmissions_StudentTest_StudentTestId",
                table: "TestSubmissions",
                column: "StudentTestId",
                principalTable: "StudentTest",
                principalColumn: "StudentTestId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestQuestions_Questions_QuestionId",
                table: "TestQuestions");

            migrationBuilder.DropForeignKey(
                name: "FK_TestQuestions_Tests_TestId",
                table: "TestQuestions");

            migrationBuilder.DropForeignKey(
                name: "FK_TestSubmissions_Questions_QuestionId",
                table: "TestSubmissions");

            migrationBuilder.DropForeignKey(
                name: "FK_TestSubmissions_StudentTest_StudentTestId",
                table: "TestSubmissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TestSubmissions",
                table: "TestSubmissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TestQuestions",
                table: "TestQuestions");

            migrationBuilder.RenameTable(
                name: "TestSubmissions",
                newName: "TestSubmission");

            migrationBuilder.RenameTable(
                name: "TestQuestions",
                newName: "TestQuestion");

            migrationBuilder.RenameIndex(
                name: "IX_TestSubmissions_StudentTestId",
                table: "TestSubmission",
                newName: "IX_TestSubmission_StudentTestId");

            migrationBuilder.RenameIndex(
                name: "IX_TestSubmissions_QuestionId",
                table: "TestSubmission",
                newName: "IX_TestSubmission_QuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_TestQuestions_TestId",
                table: "TestQuestion",
                newName: "IX_TestQuestion_TestId");

            migrationBuilder.RenameIndex(
                name: "IX_TestQuestions_QuestionId",
                table: "TestQuestion",
                newName: "IX_TestQuestion_QuestionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TestSubmission",
                table: "TestSubmission",
                column: "SubmissionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TestQuestion",
                table: "TestQuestion",
                column: "TestQuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestQuestion_Questions_QuestionId",
                table: "TestQuestion",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "QuestionId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestQuestion_Tests_TestId",
                table: "TestQuestion",
                column: "TestId",
                principalTable: "Tests",
                principalColumn: "TestId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestSubmission_Questions_QuestionId",
                table: "TestSubmission",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "QuestionId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestSubmission_StudentTest_StudentTestId",
                table: "TestSubmission",
                column: "StudentTestId",
                principalTable: "StudentTest",
                principalColumn: "StudentTestId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
