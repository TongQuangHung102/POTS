using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class UpdateTableUserParentStudent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserParentStudent",
                table: "UserParentStudent");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserParentStudent",
                table: "UserParentStudent",
                column: "UserParentStudentId");

            migrationBuilder.CreateIndex(
                name: "IX_UserParentStudent_StudentId",
                table: "UserParentStudent",
                column: "StudentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserParentStudent",
                table: "UserParentStudent");

            migrationBuilder.DropIndex(
                name: "IX_UserParentStudent_StudentId",
                table: "UserParentStudent");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserParentStudent",
                table: "UserParentStudent",
                columns: new[] { "StudentId", "ParentId" });
        }
    }
}
