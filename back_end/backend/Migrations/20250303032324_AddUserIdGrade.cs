using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class AddUserIdGrade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Grades",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Grades_UserId",
                table: "Grades",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Grades_Users_UserId",
                table: "Grades",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Grades_Users_UserId",
                table: "Grades");

            migrationBuilder.DropIndex(
                name: "IX_Grades_UserId",
                table: "Grades");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Grades");
        }
    }
}
