using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class AddDurationInMinutesToTest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prerequisite_Test_TestId",
                table: "Prerequisite");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentTest_Test_TestId",
                table: "StudentTest");

            migrationBuilder.DropForeignKey(
                name: "FK_TestQuestion_Test_TestId",
                table: "TestQuestion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Test",
                table: "Test");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Test");

            migrationBuilder.RenameTable(
                name: "Test",
                newName: "Tests");

            migrationBuilder.AddColumn<int>(
                name: "DurationInMinutes",
                table: "Tests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tests",
                table: "Tests",
                column: "TestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Prerequisite_Tests_TestId",
                table: "Prerequisite",
                column: "TestId",
                principalTable: "Tests",
                principalColumn: "TestId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentTest_Tests_TestId",
                table: "StudentTest",
                column: "TestId",
                principalTable: "Tests",
                principalColumn: "TestId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestQuestion_Tests_TestId",
                table: "TestQuestion",
                column: "TestId",
                principalTable: "Tests",
                principalColumn: "TestId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prerequisite_Tests_TestId",
                table: "Prerequisite");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentTest_Tests_TestId",
                table: "StudentTest");

            migrationBuilder.DropForeignKey(
                name: "FK_TestQuestion_Tests_TestId",
                table: "TestQuestion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tests",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "DurationInMinutes",
                table: "Tests");

            migrationBuilder.RenameTable(
                name: "Tests",
                newName: "Test");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "Duration",
                table: "Test",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Test",
                table: "Test",
                column: "TestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Prerequisite_Test_TestId",
                table: "Prerequisite",
                column: "TestId",
                principalTable: "Test",
                principalColumn: "TestId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentTest_Test_TestId",
                table: "StudentTest",
                column: "TestId",
                principalTable: "Test",
                principalColumn: "TestId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestQuestion_Test_TestId",
                table: "TestQuestion",
                column: "TestId",
                principalTable: "Test",
                principalColumn: "TestId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
