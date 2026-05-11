using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FanaCRM.Migrations
{
    /// <inheritdoc />
    public partial class AddLossReasonToOpportunity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsClosed",
                table: "OpportunityStages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsWon",
                table: "OpportunityStages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "OpportunityStages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "LossReason",
                table: "Opportunities",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "OpportunityStages",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "IsClosed", "IsWon", "Order" },
                values: new object[] { false, false, 0 });

            migrationBuilder.UpdateData(
                table: "OpportunityStages",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "IsClosed", "IsWon", "Order" },
                values: new object[] { false, false, 0 });

            migrationBuilder.UpdateData(
                table: "OpportunityStages",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "IsClosed", "IsWon", "Order" },
                values: new object[] { false, false, 0 });

            migrationBuilder.UpdateData(
                table: "OpportunityStages",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "IsClosed", "IsWon", "Order" },
                values: new object[] { false, false, 0 });

            migrationBuilder.UpdateData(
                table: "OpportunityStages",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "IsClosed", "IsWon", "Order" },
                values: new object[] { false, false, 0 });

            migrationBuilder.UpdateData(
                table: "OpportunityStages",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "IsClosed", "IsWon", "Order" },
                values: new object[] { false, false, 0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsClosed",
                table: "OpportunityStages");

            migrationBuilder.DropColumn(
                name: "IsWon",
                table: "OpportunityStages");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "OpportunityStages");

            migrationBuilder.DropColumn(
                name: "LossReason",
                table: "Opportunities");
        }
    }
}
