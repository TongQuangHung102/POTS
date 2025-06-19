using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class Fix_PaymentPlanRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_SubscriptionPlans_SubscriptionPlanPlanId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_SubscriptionPlanPlanId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "SubscriptionPlanPlanId",
                table: "Payments");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SubscriptionPlanPlanId",
                table: "Payments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_SubscriptionPlanPlanId",
                table: "Payments",
                column: "SubscriptionPlanPlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_SubscriptionPlans_SubscriptionPlanPlanId",
                table: "Payments",
                column: "SubscriptionPlanPlanId",
                principalTable: "SubscriptionPlans",
                principalColumn: "PlanId");
        }
    }
}
