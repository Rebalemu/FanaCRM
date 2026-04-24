using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FanaCRM.Migrations
{
    /// <inheritdoc />
    public partial class UpdateActivityTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_AspNetUsers_UsersId",
                table: "Activities");

            migrationBuilder.DropForeignKey(
                name: "FK_Activities_Companies_AccountId",
                table: "Activities");

            migrationBuilder.DropIndex(
                name: "IX_Activities_UsersId",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "UsersId",
                table: "Activities");

            migrationBuilder.RenameColumn(
                name: "AccountId",
                table: "Activities",
                newName: "CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_Activities_AccountId",
                table: "Activities",
                newName: "IX_Activities_CompanyId");

            migrationBuilder.AlterColumn<string>(
                name: "AssignedTo",
                table: "Activities",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_AssignedTo",
                table: "Activities",
                column: "AssignedTo");

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

            migrationBuilder.DropIndex(
                name: "IX_Activities_AssignedTo",
                table: "Activities");

            migrationBuilder.RenameColumn(
                name: "CompanyId",
                table: "Activities",
                newName: "AccountId");

            migrationBuilder.RenameIndex(
                name: "IX_Activities_CompanyId",
                table: "Activities",
                newName: "IX_Activities_AccountId");

            migrationBuilder.AlterColumn<string>(
                name: "AssignedTo",
                table: "Activities",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "UsersId",
                table: "Activities",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Activities_UsersId",
                table: "Activities",
                column: "UsersId");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_AspNetUsers_UsersId",
                table: "Activities",
                column: "UsersId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_Companies_AccountId",
                table: "Activities",
                column: "AccountId",
                principalTable: "Companies",
                principalColumn: "Id");
        }
    }
}
