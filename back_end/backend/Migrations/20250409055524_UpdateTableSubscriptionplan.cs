using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class UpdateTableSubscriptionplan : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAIAnalysis",
                table: "SubscriptionPlans");

            migrationBuilder.DropColumn(
                name: "IsAdvancedStatistics",
                table: "SubscriptionPlans");

            migrationBuilder.DropColumn(
                name: "IsBasicStatistics",
                table: "SubscriptionPlans");

            migrationBuilder.DropColumn(
                name: "IsPersonalization",
                table: "SubscriptionPlans");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAIAnalysis",
                table: "SubscriptionPlans",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsAdvancedStatistics",
                table: "SubscriptionPlans",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsBasicStatistics",
                table: "SubscriptionPlans",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPersonalization",
                table: "SubscriptionPlans",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
