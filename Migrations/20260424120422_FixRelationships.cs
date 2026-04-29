using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FanaCRM.Migrations
{
    /// <inheritdoc />
    public partial class FixRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_AspNetUsers_AssignedTo",
                table: "Activities");

            migrationBuilder.DropForeignKey(
                name: "FK_Activities_Companies_CompanyId",
                table: "Activities");

            migrationBuilder.DropForeignKey(
                name: "FK_Activities_Contacts_ContactId",
                table: "Activities");

            migrationBuilder.DropForeignKey(
                name: "FK_Contacts_Companies_CompanyId",
                table: "Contacts");

            migrationBuilder.DropForeignKey(
                name: "FK_Leads_AspNetUsers_AssignedTo",
                table: "Leads");

            migrationBuilder.DropForeignKey(
                name: "FK_Opportunities_AspNetUsers_AssignedTo",
                table: "Opportunities");

            migrationBuilder.DropForeignKey(
                name: "FK_Opportunities_OpportunityStages_StageId",
                table: "Opportunities");

            migrationBuilder.DropForeignKey(
                name: "FK_OpportunityProducts_Products_ProductId",
                table: "OpportunityProducts");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_AspNetUsers_AssignedTo",
                table: "Activities",
                column: "AssignedTo",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_Companies_CompanyId",
                table: "Activities",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_Contacts_ContactId",
                table: "Activities",
                column: "ContactId",
                principalTable: "Contacts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Contacts_Companies_CompanyId",
                table: "Contacts",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Leads_AspNetUsers_AssignedTo",
                table: "Leads",
                column: "AssignedTo",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Opportunities_AspNetUsers_AssignedTo",
                table: "Opportunities",
                column: "AssignedTo",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Opportunities_OpportunityStages_StageId",
                table: "Opportunities",
                column: "StageId",
                principalTable: "OpportunityStages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OpportunityProducts_Products_ProductId",
                table: "OpportunityProducts",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_AspNetUsers_AssignedTo",
                table: "Activities");

            migrationBuilder.DropForeignKey(
                name: "FK_Activities_Companies_CompanyId",
                table: "Activities");

            migrationBuilder.DropForeignKey(
                name: "FK_Activities_Contacts_ContactId",
                table: "Activities");

            migrationBuilder.DropForeignKey(
                name: "FK_Contacts_Companies_CompanyId",
                table: "Contacts");

            migrationBuilder.DropForeignKey(
                name: "FK_Leads_AspNetUsers_AssignedTo",
                table: "Leads");

            migrationBuilder.DropForeignKey(
                name: "FK_Opportunities_AspNetUsers_AssignedTo",
                table: "Opportunities");

            migrationBuilder.DropForeignKey(
                name: "FK_Opportunities_OpportunityStages_StageId",
                table: "Opportunities");

            migrationBuilder.DropForeignKey(
                name: "FK_OpportunityProducts_Products_ProductId",
                table: "OpportunityProducts");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_AspNetUsers_AssignedTo",
                table: "Activities",
                column: "AssignedTo",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_Companies_CompanyId",
                table: "Activities",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_Contacts_ContactId",
                table: "Activities",
                column: "ContactId",
                principalTable: "Contacts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Contacts_Companies_CompanyId",
                table: "Contacts",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Leads_AspNetUsers_AssignedTo",
                table: "Leads",
                column: "AssignedTo",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Opportunities_AspNetUsers_AssignedTo",
                table: "Opportunities",
                column: "AssignedTo",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Opportunities_OpportunityStages_StageId",
                table: "Opportunities",
                column: "StageId",
                principalTable: "OpportunityStages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OpportunityProducts_Products_ProductId",
                table: "OpportunityProducts",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
