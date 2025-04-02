using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class UpdateStudentTest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentTest_Tests_TestId",
                table: "StudentTest");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentTest_Users_UserId",
                table: "StudentTest");

            migrationBuilder.DropForeignKey(
                name: "FK_TestSubmissions_StudentTest_StudentTestId",
                table: "TestSubmissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentTest",
                table: "StudentTest");

            migrationBuilder.RenameTable(
                name: "StudentTest",
                newName: "StudentTests");

            migrationBuilder.RenameIndex(
                name: "IX_StudentTest_UserId",
                table: "StudentTests",
                newName: "IX_StudentTests_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentTest_TestId",
                table: "StudentTests",
                newName: "IX_StudentTests_TestId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentTests",
                table: "StudentTests",
                column: "StudentTestId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentTests_Tests_TestId",
                table: "StudentTests",
                column: "TestId",
                principalTable: "Tests",
                principalColumn: "TestId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentTests_Users_UserId",
                table: "StudentTests",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestSubmissions_StudentTests_StudentTestId",
                table: "TestSubmissions",
                column: "StudentTestId",
                principalTable: "StudentTests",
                principalColumn: "StudentTestId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentTests_Tests_TestId",
                table: "StudentTests");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentTests_Users_UserId",
                table: "StudentTests");

            migrationBuilder.DropForeignKey(
                name: "FK_TestSubmissions_StudentTests_StudentTestId",
                table: "TestSubmissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentTests",
                table: "StudentTests");

            migrationBuilder.RenameTable(
                name: "StudentTests",
                newName: "StudentTest");

            migrationBuilder.RenameIndex(
                name: "IX_StudentTests_UserId",
                table: "StudentTest",
                newName: "IX_StudentTest_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentTests_TestId",
                table: "StudentTest",
                newName: "IX_StudentTest_TestId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentTest",
                table: "StudentTest",
                column: "StudentTestId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentTest_Tests_TestId",
                table: "StudentTest",
                column: "TestId",
                principalTable: "Tests",
                principalColumn: "TestId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentTest_Users_UserId",
                table: "StudentTest",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestSubmissions_StudentTest_StudentTestId",
                table: "TestSubmissions",
                column: "StudentTestId",
                principalTable: "StudentTest",
                principalColumn: "StudentTestId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
