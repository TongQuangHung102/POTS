using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class AddTestChapterRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prerequisite_Lessons_LessonId",
                table: "Prerequisite");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Grades_GradeId",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "LessonId",
                table: "Prerequisite",
                newName: "ChapterId");

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "Test",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Semester",
                table: "Chapters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "TestCategories",
                columns: table => new
                {
                    TestCategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestCategories", x => x.TestCategoryId);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Prerequisite_Chapters_ChapterId",
                table: "Prerequisite",
                column: "ChapterId",
                principalTable: "Chapters",
                principalColumn: "ChapterId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Grades_GradeId",
                table: "Users",
                column: "GradeId",
                principalTable: "Grades",
                principalColumn: "GradeId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prerequisite_Chapters_ChapterId",
                table: "Prerequisite");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Grades_GradeId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "TestCategories");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "Test");

            migrationBuilder.DropColumn(
                name: "Semester",
                table: "Chapters");

            migrationBuilder.RenameColumn(
                name: "ChapterId",
                table: "Prerequisite",
                newName: "LessonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Prerequisite_Lessons_LessonId",
                table: "Prerequisite",
                column: "LessonId",
                principalTable: "Lessons",
                principalColumn: "LessonId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Grades_GradeId",
                table: "Users",
                column: "GradeId",
                principalTable: "Grades",
                principalColumn: "GradeId",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
