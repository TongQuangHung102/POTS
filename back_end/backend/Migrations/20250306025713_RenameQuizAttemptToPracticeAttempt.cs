using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class RenameQuizAttemptToPracticeAttempt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentAnswers_QuizAttempts_AttemptId",
                table: "StudentAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentAnswers_QuizAttempts_QuizAttemptAttemptId",
                table: "StudentAnswers");

            migrationBuilder.DropTable(
                name: "QuizAttempts");

            migrationBuilder.RenameColumn(
                name: "QuizAttemptAttemptId",
                table: "StudentAnswers",
                newName: "PracticeAttemptPracticeId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentAnswers_QuizAttemptAttemptId",
                table: "StudentAnswers",
                newName: "IX_StudentAnswers_PracticeAttemptPracticeId");

            migrationBuilder.CreateTable(
                name: "PracticeAttempts",
                columns: table => new
                {
                    PracticeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CorrectAnswers = table.Column<int>(type: "int", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    Time = table.Column<TimeSpan>(type: "time", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LessonId = table.Column<int>(type: "int", nullable: false),
                    LevelId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PracticeAttempts", x => x.PracticeId);
                    table.ForeignKey(
                        name: "FK_PracticeAttempts_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "LessonId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PracticeAttempts_Levels_LevelId",
                        column: x => x.LevelId,
                        principalTable: "Levels",
                        principalColumn: "LevelId");
                    table.ForeignKey(
                        name: "FK_PracticeAttempts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PracticeAttempts_LessonId",
                table: "PracticeAttempts",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_PracticeAttempts_LevelId",
                table: "PracticeAttempts",
                column: "LevelId");

            migrationBuilder.CreateIndex(
                name: "IX_PracticeAttempts_UserId",
                table: "PracticeAttempts",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAnswers_PracticeAttempts_AttemptId",
                table: "StudentAnswers",
                column: "AttemptId",
                principalTable: "PracticeAttempts",
                principalColumn: "PracticeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAnswers_PracticeAttempts_PracticeAttemptPracticeId",
                table: "StudentAnswers",
                column: "PracticeAttemptPracticeId",
                principalTable: "PracticeAttempts",
                principalColumn: "PracticeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentAnswers_PracticeAttempts_AttemptId",
                table: "StudentAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentAnswers_PracticeAttempts_PracticeAttemptPracticeId",
                table: "StudentAnswers");

            migrationBuilder.DropTable(
                name: "PracticeAttempts");

            migrationBuilder.RenameColumn(
                name: "PracticeAttemptPracticeId",
                table: "StudentAnswers",
                newName: "QuizAttemptAttemptId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentAnswers_PracticeAttemptPracticeId",
                table: "StudentAnswers",
                newName: "IX_StudentAnswers_QuizAttemptAttemptId");

            migrationBuilder.CreateTable(
                name: "QuizAttempts",
                columns: table => new
                {
                    AttemptId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LessonId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CorrectAnswers = table.Column<int>(type: "int", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    LevelId = table.Column<int>(type: "int", nullable: true),
                    Time = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizAttempts", x => x.AttemptId);
                    table.ForeignKey(
                        name: "FK_QuizAttempts_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "LessonId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuizAttempts_Levels_LevelId",
                        column: x => x.LevelId,
                        principalTable: "Levels",
                        principalColumn: "LevelId");
                    table.ForeignKey(
                        name: "FK_QuizAttempts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuizAttempts_LessonId",
                table: "QuizAttempts",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizAttempts_LevelId",
                table: "QuizAttempts",
                column: "LevelId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizAttempts_UserId",
                table: "QuizAttempts",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAnswers_QuizAttempts_AttemptId",
                table: "StudentAnswers",
                column: "AttemptId",
                principalTable: "QuizAttempts",
                principalColumn: "AttemptId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAnswers_QuizAttempts_QuizAttemptAttemptId",
                table: "StudentAnswers",
                column: "QuizAttemptAttemptId",
                principalTable: "QuizAttempts",
                principalColumn: "AttemptId");
        }
    }
}
