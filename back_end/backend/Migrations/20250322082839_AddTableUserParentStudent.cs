using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class AddTableUserParentStudent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserParentStudent_Users_ParentId",
                table: "UserParentStudent");

            migrationBuilder.DropForeignKey(
                name: "FK_UserParentStudent_Users_StudentId",
                table: "UserParentStudent");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserParentStudent",
                table: "UserParentStudent");

            migrationBuilder.RenameTable(
                name: "UserParentStudent",
                newName: "UserParentStudents");

            migrationBuilder.RenameIndex(
                name: "IX_UserParentStudent_StudentId",
                table: "UserParentStudents",
                newName: "IX_UserParentStudents_StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_UserParentStudent_ParentId",
                table: "UserParentStudents",
                newName: "IX_UserParentStudents_ParentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserParentStudents",
                table: "UserParentStudents",
                column: "UserParentStudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserParentStudents_Users_ParentId",
                table: "UserParentStudents",
                column: "ParentId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserParentStudents_Users_StudentId",
                table: "UserParentStudents",
                column: "StudentId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserParentStudents_Users_ParentId",
                table: "UserParentStudents");

            migrationBuilder.DropForeignKey(
                name: "FK_UserParentStudents_Users_StudentId",
                table: "UserParentStudents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserParentStudents",
                table: "UserParentStudents");

            migrationBuilder.RenameTable(
                name: "UserParentStudents",
                newName: "UserParentStudent");

            migrationBuilder.RenameIndex(
                name: "IX_UserParentStudents_StudentId",
                table: "UserParentStudent",
                newName: "IX_UserParentStudent_StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_UserParentStudents_ParentId",
                table: "UserParentStudent",
                newName: "IX_UserParentStudent_ParentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserParentStudent",
                table: "UserParentStudent",
                column: "UserParentStudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserParentStudent_Users_ParentId",
                table: "UserParentStudent",
                column: "ParentId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserParentStudent_Users_StudentId",
                table: "UserParentStudent",
                column: "StudentId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
