using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class AddGradeForeignKeyToTests : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Thêm cột GradeId vào bảng Tests nếu chưa có
            migrationBuilder.AddColumn<int>(
                name: "GradeId",
                table: "Tests",
                nullable: false,
                defaultValue: 1);

            // Thêm khóa ngoại giữa Tests và Grades
            migrationBuilder.CreateIndex(
                name: "IX_Tests_GradeId",
                table: "Tests",
                column: "GradeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tests_Grades_GradeId",
                table: "Tests",
                column: "GradeId",
                principalTable: "Grades",
                principalColumn: "GradeId",
                onDelete: ReferentialAction.Restrict); // Xóa grade sẽ xóa luôn test liên quan
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
            name: "FK_Tests_Grades_GradeId",
            table: "Tests");

            // Xóa index trên GradeId
            migrationBuilder.DropIndex(
                name: "IX_Tests_GradeId",
                table: "Tests");

            // Xóa cột GradeId
            migrationBuilder.DropColumn(
                name: "GradeId",
                table: "Tests");

            // Xóa bảng Grades nếu không còn cần thiết
            migrationBuilder.DropTable(
                name: "Grades");
        }
    }
}
