using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FanaCRM.Migrations
{
    /// <inheritdoc />
    public partial class AddActivityIdAndCreatedDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "TimelineEvents");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "TimelineEvents",
                newName: "CreatedDate");

            migrationBuilder.RenameColumn(
                name: "ContactId",
                table: "TimelineEvents",
                newName: "ActivityId");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "TimelineEvents",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "TimelineEvents",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "EventType",
                table: "TimelineEvents",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_TimelineEvents_ActivityId",
                table: "TimelineEvents",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_TimelineEvents_LeadId",
                table: "TimelineEvents",
                column: "LeadId");

            migrationBuilder.CreateIndex(
                name: "IX_TimelineEvents_OpportunityId",
                table: "TimelineEvents",
                column: "OpportunityId");

            migrationBuilder.CreateIndex(
                name: "IX_TimelineEvents_UserId",
                table: "TimelineEvents",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TimelineEvents_Activities_ActivityId",
                table: "TimelineEvents",
                column: "ActivityId",
                principalTable: "Activities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TimelineEvents_AspNetUsers_UserId",
                table: "TimelineEvents",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TimelineEvents_Leads_LeadId",
                table: "TimelineEvents",
                column: "LeadId",
                principalTable: "Leads",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TimelineEvents_Opportunities_OpportunityId",
                table: "TimelineEvents",
                column: "OpportunityId",
                principalTable: "Opportunities",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TimelineEvents_Activities_ActivityId",
                table: "TimelineEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_TimelineEvents_AspNetUsers_UserId",
                table: "TimelineEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_TimelineEvents_Leads_LeadId",
                table: "TimelineEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_TimelineEvents_Opportunities_OpportunityId",
                table: "TimelineEvents");

            migrationBuilder.DropIndex(
                name: "IX_TimelineEvents_ActivityId",
                table: "TimelineEvents");

            migrationBuilder.DropIndex(
                name: "IX_TimelineEvents_LeadId",
                table: "TimelineEvents");

            migrationBuilder.DropIndex(
                name: "IX_TimelineEvents_OpportunityId",
                table: "TimelineEvents");

            migrationBuilder.DropIndex(
                name: "IX_TimelineEvents_UserId",
                table: "TimelineEvents");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "TimelineEvents",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "ActivityId",
                table: "TimelineEvents",
                newName: "ContactId");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "TimelineEvents",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "TimelineEvents",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "EventType",
                table: "TimelineEvents",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "TimelineEvents",
                type: "int",
                nullable: true);
        }
    }
}
