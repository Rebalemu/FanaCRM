using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FanaCRM.Migrations
{
    /// <inheritdoc />
    public partial class SeedProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Description", "IsActive", "Name", "Price" },
                values: new object[,]
                {
                    { 1, "13-inch ultrabook, 16GB RAM, 512GB SSD", true, "Laptop - Dell XPS 13", 1200.00m },
                    { 2, "Ryzen 5, 16GB RAM, 1TB SSD, GTX 1660", true, "Desktop PC - Custom Build", 950.00m },
                    { 3, "Ergonomic wireless mouse", true, "Wireless Mouse - Logitech", 25.50m },
                    { 4, "RGB backlit mechanical keyboard", true, "Mechanical Keyboard", 75.00m },
                    { 5, "Ultra HD IPS display", true, "27-inch Monitor - 4K", 300.00m },
                    { 6, "Portable high-speed storage", true, "External SSD 1TB", 150.00m },
                    { 7, "Multiport adapter with HDMI, USB 3.0", true, "USB-C Hub", 40.00m },
                    { 8, "Surround sound headset with mic", true, "Gaming Headset", 60.00m },
                    { 9, "Full HD webcam for streaming and meetings", true, "Webcam HD 1080p", 45.00m },
                    { 10, "1-year subscription license", true, "Office Software License", 120.00m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 10);
        }
    }
}
