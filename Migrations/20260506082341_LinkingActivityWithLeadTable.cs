using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FanaCRM.Migrations
{
    /// <inheritdoc />
    public partial class LinkingActivityWithLeadTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastActivityDate",
                table: "Leads",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastContactedDate",
                table: "Leads",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LeadId",
                table: "Activities",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Activities_LeadId",
                table: "Activities",
                column: "LeadId");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_Leads_LeadId",
                table: "Activities",
                column: "LeadId",
                principalTable: "Leads",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_Leads_LeadId",
                table: "Activities");

            migrationBuilder.DropIndex(
                name: "IX_Activities_LeadId",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "LastActivityDate",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "LastContactedDate",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "LeadId",
                table: "Activities");
        }
    }
}
