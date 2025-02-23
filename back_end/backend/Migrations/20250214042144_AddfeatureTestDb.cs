using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class AddfeatureTestDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "QuizAttemptAttemptId",
                table: "StudentAnswers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LevelId",
                table: "QuizAttempts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SubscriptionPlanPlanId",
                table: "Payments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Test",
                columns: table => new
                {
                    TestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Duration = table.Column<TimeSpan>(type: "time", nullable: false),
                    MaxScore = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Test", x => x.TestId);
                });

            migrationBuilder.CreateTable(
                name: "Prerequisite",
                columns: table => new
                {
                    LessonId = table.Column<int>(type: "int", nullable: false),
                    TestId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prerequisite", x => new { x.LessonId, x.TestId });
                    table.ForeignKey(
                        name: "FK_Prerequisite_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "LessonId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Prerequisite_Test_TestId",
                        column: x => x.TestId,
                        principalTable: "Test",
                        principalColumn: "TestId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentTest",
                columns: table => new
                {
                    StudentTestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Score = table.Column<double>(type: "float", nullable: true),
                    TestId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentTest", x => x.StudentTestId);
                    table.ForeignKey(
                        name: "FK_StudentTest_Test_TestId",
                        column: x => x.TestId,
                        principalTable: "Test",
                        principalColumn: "TestId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentTest_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestQuestion",
                columns: table => new
                {
                    TestQuestionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    TestId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestQuestion", x => x.TestQuestionId);
                    table.ForeignKey(
                        name: "FK_TestQuestion_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "QuestionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestQuestion_Test_TestId",
                        column: x => x.TestId,
                        principalTable: "Test",
                        principalColumn: "TestId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestSubmission",
                columns: table => new
                {
                    SubmissionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentAnswer = table.Column<int>(type: "int", nullable: true),
                    StudentTestId = table.Column<int>(type: "int", nullable: false),
                    QuestionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestSubmission", x => x.SubmissionId);
                    table.ForeignKey(
                        name: "FK_TestSubmission_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "QuestionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestSubmission_StudentTest_StudentTestId",
                        column: x => x.StudentTestId,
                        principalTable: "StudentTest",
                        principalColumn: "StudentTestId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentAnswers_QuizAttemptAttemptId",
                table: "StudentAnswers",
                column: "QuizAttemptAttemptId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizAttempts_LevelId",
                table: "QuizAttempts",
                column: "LevelId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_SubscriptionPlanPlanId",
                table: "Payments",
                column: "SubscriptionPlanPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_Prerequisite_TestId",
                table: "Prerequisite",
                column: "TestId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentTest_TestId",
                table: "StudentTest",
                column: "TestId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentTest_UserId",
                table: "StudentTest",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TestQuestion_QuestionId",
                table: "TestQuestion",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_TestQuestion_TestId",
                table: "TestQuestion",
                column: "TestId");

            migrationBuilder.CreateIndex(
                name: "IX_TestSubmission_QuestionId",
                table: "TestSubmission",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_TestSubmission_StudentTestId",
                table: "TestSubmission",
                column: "StudentTestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_SubscriptionPlans_SubscriptionPlanPlanId",
                table: "Payments",
                column: "SubscriptionPlanPlanId",
                principalTable: "SubscriptionPlans",
                principalColumn: "PlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuizAttempts_Levels_LevelId",
                table: "QuizAttempts",
                column: "LevelId",
                principalTable: "Levels",
                principalColumn: "LevelId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAnswers_QuizAttempts_QuizAttemptAttemptId",
                table: "StudentAnswers",
                column: "QuizAttemptAttemptId",
                principalTable: "QuizAttempts",
                principalColumn: "AttemptId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_SubscriptionPlans_SubscriptionPlanPlanId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizAttempts_Levels_LevelId",
                table: "QuizAttempts");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentAnswers_QuizAttempts_QuizAttemptAttemptId",
                table: "StudentAnswers");

            migrationBuilder.DropTable(
                name: "Prerequisite");

            migrationBuilder.DropTable(
                name: "TestQuestion");

            migrationBuilder.DropTable(
                name: "TestSubmission");

            migrationBuilder.DropTable(
                name: "StudentTest");

            migrationBuilder.DropTable(
                name: "Test");

            migrationBuilder.DropIndex(
                name: "IX_StudentAnswers_QuizAttemptAttemptId",
                table: "StudentAnswers");

            migrationBuilder.DropIndex(
                name: "IX_QuizAttempts_LevelId",
                table: "QuizAttempts");

            migrationBuilder.DropIndex(
                name: "IX_Payments_SubscriptionPlanPlanId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "QuizAttemptAttemptId",
                table: "StudentAnswers");

            migrationBuilder.DropColumn(
                name: "LevelId",
                table: "QuizAttempts");

            migrationBuilder.DropColumn(
                name: "SubscriptionPlanPlanId",
                table: "Payments");

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
