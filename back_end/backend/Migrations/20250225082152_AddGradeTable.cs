using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class AddGradeTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Tạo bảng Grades trước
            migrationBuilder.CreateTable(
                name: "Grades",
                columns: table => new
                {
                    GradeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GradeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grades", x => x.GradeId);
                });

            // Thêm giá trị mặc định vào bảng Grades để tránh lỗi
            migrationBuilder.InsertData(
                table: "Grades",
                columns: new[] { "GradeId", "GradeName", "Description", "IsVisible" },
                values: new object[] { 1, "Default Grade", "This is a default grade", true });

            // Thêm cột GradeId vào bảng Chapters
            migrationBuilder.AddColumn<int>(
                name: "GradeId",
                table: "Chapters",
                type: "int",
                nullable: false,
                defaultValue: 1); // Đặt giá trị mặc định là 1 để tham chiếu đến Grade vừa tạo

            // Tạo Index cho GradeId
            migrationBuilder.CreateIndex(
                name: "IX_Chapters_GradeId",
                table: "Chapters",
                column: "GradeId");

            // Thêm khóa ngoại
            migrationBuilder.AddForeignKey(
                name: "FK_Chapters_Grades_GradeId",
                table: "Chapters",
                column: "GradeId",
                principalTable: "Grades",
                principalColumn: "GradeId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chapters_Grades_GradeId",
                table: "Chapters");

            migrationBuilder.DropTable(
                name: "Grades");

            migrationBuilder.DropIndex(
                name: "IX_Chapters_GradeId",
                table: "Chapters");

            migrationBuilder.DropColumn(
                name: "GradeId",
                table: "Chapters");
        }
    }
}
