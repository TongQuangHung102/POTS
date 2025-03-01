using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class UpdateDurationToInt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DurationTemp",
                table: "SubscriptionPlans",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(
                "UPDATE SubscriptionPlans " +
                "SET DurationTemp = DATEDIFF(MONTH, GETDATE(), Duration)");


            migrationBuilder.DropColumn(
                name: "Duration",
                table: "SubscriptionPlans");

 
            migrationBuilder.RenameColumn(
                name: "DurationTemp",
                table: "SubscriptionPlans",
                newName: "Duration");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Duration",
                table: "SubscriptionPlans",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.Sql(
                "UPDATE SubscriptionPlans " +
                "SET Duration = DATEADD(MONTH, Duration, GETDATE())");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "SubscriptionPlans");
        }
    }
}
