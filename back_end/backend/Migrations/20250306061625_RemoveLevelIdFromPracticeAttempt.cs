using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class RemoveLevelIdFromPracticeAttempt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PracticeAttempts_Levels_LevelId",
                table: "PracticeAttempts");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "PracticeAttempts");

            migrationBuilder.AlterColumn<int>(
                name: "LevelId",
                table: "PracticeAttempts",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PracticeAttempts_Levels_LevelId",
                table: "PracticeAttempts",
                column: "LevelId",
                principalTable: "Levels",
                principalColumn: "LevelId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PracticeAttempts_Levels_LevelId",
                table: "PracticeAttempts");

            migrationBuilder.AlterColumn<int>(
                name: "LevelId",
                table: "PracticeAttempts",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "Level",
                table: "PracticeAttempts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_PracticeAttempts_Levels_LevelId",
                table: "PracticeAttempts",
                column: "LevelId",
                principalTable: "Levels",
                principalColumn: "LevelId");
        }
    }
}
