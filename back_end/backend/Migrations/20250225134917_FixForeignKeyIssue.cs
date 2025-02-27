using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class FixForeignKeyIssue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GradeId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_GradeId",
                table: "Users",
                column: "GradeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Grades_GradeId",
                table: "Users",
                column: "GradeId",
                principalTable: "Grades",
                principalColumn: "GradeId",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Grades_GradeId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_GradeId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "GradeId",
                table: "Users");
        }
    }
}
