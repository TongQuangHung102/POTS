using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class EditTableChapter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SubjectGradeId",
                table: "Chapters",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Chapters_SubjectGradeId",
                table: "Chapters",
                column: "SubjectGradeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chapters_SubjectGrades_SubjectGradeId",
                table: "Chapters",
                column: "SubjectGradeId",
                principalTable: "SubjectGrades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chapters_SubjectGrades_SubjectGradeId",
                table: "Chapters");

            migrationBuilder.DropIndex(
                name: "IX_Chapters_SubjectGradeId",
                table: "Chapters");

            migrationBuilder.DropColumn(
                name: "SubjectGradeId",
                table: "Chapters");
        }
    }
}
