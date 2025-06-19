using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class AddTablePracticeQuestion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentAnswers_Questions_QuestionId",
                table: "StudentAnswers");

            migrationBuilder.AddColumn<int>(
                name: "QuestionId1",
                table: "StudentAnswers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PracticeQuestions",
                columns: table => new
                {
                    QuestionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LevelId = table.Column<int>(type: "int", nullable: false),
                    CorrectAnswer = table.Column<int>(type: "int", nullable: false),
                    LessonId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PracticeQuestions", x => x.QuestionId);
                    table.ForeignKey(
                        name: "FK_PracticeQuestions_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "LessonId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PracticeQuestions_Levels_LevelId",
                        column: x => x.LevelId,
                        principalTable: "Levels",
                        principalColumn: "LevelId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnswerPracticeQuestions",
                columns: table => new
                {
                    AnswerQuestionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnswerText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Number = table.Column<int>(type: "int", nullable: false),
                    QuestionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnswerPracticeQuestions", x => x.AnswerQuestionId);
                    table.ForeignKey(
                        name: "FK_AnswerPracticeQuestions_PracticeQuestions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "PracticeQuestions",
                        principalColumn: "QuestionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentAnswers_QuestionId1",
                table: "StudentAnswers",
                column: "QuestionId1");

            migrationBuilder.CreateIndex(
                name: "IX_AnswerPracticeQuestions_QuestionId",
                table: "AnswerPracticeQuestions",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_PracticeQuestions_LessonId",
                table: "PracticeQuestions",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_PracticeQuestions_LevelId",
                table: "PracticeQuestions",
                column: "LevelId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAnswers_PracticeQuestions_QuestionId",
                table: "StudentAnswers",
                column: "QuestionId",
                principalTable: "PracticeQuestions",
                principalColumn: "QuestionId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAnswers_Questions_QuestionId1",
                table: "StudentAnswers",
                column: "QuestionId1",
                principalTable: "Questions",
                principalColumn: "QuestionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentAnswers_PracticeQuestions_QuestionId",
                table: "StudentAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentAnswers_Questions_QuestionId1",
                table: "StudentAnswers");

            migrationBuilder.DropTable(
                name: "AnswerPracticeQuestions");

            migrationBuilder.DropTable(
                name: "PracticeQuestions");

            migrationBuilder.DropIndex(
                name: "IX_StudentAnswers_QuestionId1",
                table: "StudentAnswers");

            migrationBuilder.DropColumn(
                name: "QuestionId1",
                table: "StudentAnswers");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAnswers_Questions_QuestionId",
                table: "StudentAnswers",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "QuestionId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
