using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class edidTableChapter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chapters_Grades_GradeId",
                table: "Chapters");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SubjectGrades",
                table: "SubjectGrades");

            migrationBuilder.DropIndex(
                name: "IX_Chapters_GradeId",
                table: "Chapters");

            migrationBuilder.DropColumn(
                name: "GradeId",
                table: "Chapters");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "SubjectGrades",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubjectGrades",
                table: "SubjectGrades",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectGrades_SubjectId",
                table: "SubjectGrades",
                column: "SubjectId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SubjectGrades",
                table: "SubjectGrades");

            migrationBuilder.DropIndex(
                name: "IX_SubjectGrades_SubjectId",
                table: "SubjectGrades");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "SubjectGrades");

            migrationBuilder.AddColumn<int>(
                name: "GradeId",
                table: "Chapters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubjectGrades",
                table: "SubjectGrades",
                columns: new[] { "SubjectId", "GradeId" });

            migrationBuilder.CreateIndex(
                name: "IX_Chapters_GradeId",
                table: "Chapters",
                column: "GradeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chapters_Grades_GradeId",
                table: "Chapters",
                column: "GradeId",
                principalTable: "Grades",
                principalColumn: "GradeId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
