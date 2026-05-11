using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FanaCRM.Migrations
{
    /// <inheritdoc />
    public partial class AddOpportunityHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OpportunityStageHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OpportunityId = table.Column<int>(type: "int", nullable: false),
                    StageId = table.Column<int>(type: "int", nullable: false),
                    EnteredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExitedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ChangedByUserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpportunityStageHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OpportunityStageHistories_AspNetUsers_ChangedByUserId",
                        column: x => x.ChangedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OpportunityStageHistories_Opportunities_OpportunityId",
                        column: x => x.OpportunityId,
                        principalTable: "Opportunities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OpportunityStageHistories_OpportunityStages_StageId",
                        column: x => x.StageId,
                        principalTable: "OpportunityStages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OpportunityStageHistories_ChangedByUserId",
                table: "OpportunityStageHistories",
                column: "ChangedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OpportunityStageHistories_OpportunityId",
                table: "OpportunityStageHistories",
                column: "OpportunityId");

            migrationBuilder.CreateIndex(
                name: "IX_OpportunityStageHistories_StageId",
                table: "OpportunityStageHistories",
                column: "StageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OpportunityStageHistories");
        }
    }
}
