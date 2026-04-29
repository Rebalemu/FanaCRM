using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FanaCRM.Migrations
{
    /// <inheritdoc />
    public partial class UpdateleadTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsConverted",
                table: "Leads",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsConverted",
                table: "Leads");
        }
    }
}
