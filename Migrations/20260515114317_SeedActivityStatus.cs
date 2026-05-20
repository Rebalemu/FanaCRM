using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FanaCRM.Migrations
{
    /// <inheritdoc />
    public partial class SeedActivityStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_ActivityStatus_ActivityStatusId",
                table: "Activities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ActivityStatus",
                table: "ActivityStatus");

            migrationBuilder.RenameTable(
                name: "ActivityStatus",
                newName: "ActivityStatuses");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ActivityStatuses",
                table: "ActivityStatuses",
                column: "Id");

            migrationBuilder.InsertData(
                table: "ActivityStatuses",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Pending" },
                    { 2, "In Progress" },
                    { 3, "Completed" },
                    { 4, "Cancelled" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_ActivityStatuses_ActivityStatusId",
                table: "Activities",
                column: "ActivityStatusId",
                principalTable: "ActivityStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_ActivityStatuses_ActivityStatusId",
                table: "Activities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ActivityStatuses",
                table: "ActivityStatuses");

            migrationBuilder.DeleteData(
                table: "ActivityStatuses",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ActivityStatuses",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ActivityStatuses",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ActivityStatuses",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.RenameTable(
                name: "ActivityStatuses",
                newName: "ActivityStatus");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ActivityStatus",
                table: "ActivityStatus",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_ActivityStatus_ActivityStatusId",
                table: "Activities",
                column: "ActivityStatusId",
                principalTable: "ActivityStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
