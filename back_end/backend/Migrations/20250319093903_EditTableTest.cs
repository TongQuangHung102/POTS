using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class EditTableTest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tests_Grades_GradeId",
                table: "Tests");

            migrationBuilder.RenameColumn(
                name: "GradeId",
                table: "Tests",
                newName: "SubjectGradeId");

            migrationBuilder.RenameIndex(
                name: "IX_Tests_GradeId",
                table: "Tests",
                newName: "IX_Tests_SubjectGradeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tests_SubjectGrades_SubjectGradeId",
                table: "Tests",
                column: "SubjectGradeId",
                principalTable: "SubjectGrades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tests_SubjectGrades_SubjectGradeId",
                table: "Tests");

            migrationBuilder.RenameColumn(
                name: "SubjectGradeId",
                table: "Tests",
                newName: "GradeId");

            migrationBuilder.RenameIndex(
                name: "IX_Tests_SubjectGradeId",
                table: "Tests",
                newName: "IX_Tests_GradeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tests_Grades_GradeId",
                table: "Tests",
                column: "GradeId",
                principalTable: "Grades",
                principalColumn: "GradeId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
