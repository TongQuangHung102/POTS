using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class UpdateKeyTableStudentPerformance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentPerformances",
                table: "StudentPerformances");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentPerformances",
                table: "StudentPerformances",
                columns: new[] { "UserId", "LessonId", "Month", "Year" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentPerformances",
                table: "StudentPerformances");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentPerformances",
                table: "StudentPerformances",
                columns: new[] { "UserId", "LessonId" });
        }
    }
}
