using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FanaCRM.Migrations
{
    /// <inheritdoc />
    public partial class AddOpportunityRelationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Opportunities_Leads_LeadId",
                table: "Opportunities");

            migrationBuilder.DropIndex(
                name: "IX_Opportunities_LeadId",
                table: "Opportunities");

            migrationBuilder.AddColumn<int>(
                name: "OpportunityId",
                table: "Leads",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Leads_OpportunityId",
                table: "Leads",
                column: "OpportunityId",
                unique: true,
                filter: "[OpportunityId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Leads_Opportunities_OpportunityId",
                table: "Leads",
                column: "OpportunityId",
                principalTable: "Opportunities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Leads_Opportunities_OpportunityId",
                table: "Leads");

            migrationBuilder.DropIndex(
                name: "IX_Leads_OpportunityId",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "OpportunityId",
                table: "Leads");

            migrationBuilder.CreateIndex(
                name: "IX_Opportunities_LeadId",
                table: "Opportunities",
                column: "LeadId");

            migrationBuilder.AddForeignKey(
                name: "FK_Opportunities_Leads_LeadId",
                table: "Opportunities",
                column: "LeadId",
                principalTable: "Leads",
                principalColumn: "Id");
        }
    }
}
