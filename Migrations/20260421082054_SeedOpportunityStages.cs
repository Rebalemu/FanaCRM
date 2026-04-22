using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FanaCRM.Migrations
{
    /// <inheritdoc />
    public partial class SeedOpportunityStages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "OpportunityStages",
                columns: new[] { "Id", "Name", "Probability" },
                values: new object[,]
                {
                    { 1, "Prospect", 10 },
                    { 2, "Qualified", 30 },
                    { 3, "Proposal", 60 },
                    { 4, "Negotiation", 80 },
                    { 5, "Won", 100 },
                    { 6, "Lost", 0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "OpportunityStages",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "OpportunityStages",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "OpportunityStages",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "OpportunityStages",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "OpportunityStages",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "OpportunityStages",
                keyColumn: "Id",
                keyValue: 6);
        }
    }
}
