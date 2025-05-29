using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eCommerceOnlineShop.IdentityServer.Migrations
{
    /// <inheritdoc />
    public partial class SeedRolesMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
            table: "AspNetRoles",
            columns: ["Id", "Name", "NormalizedName"],
            values: new object[,]
            {
                { "1", "Manager", "MANAGER" },
                { "2", "Customer", "CUSTOMER" }
            });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
            table: "AspNetRoles",
            keyColumn: "Id",
            keyValue: "1");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2");
        }
    }
}
