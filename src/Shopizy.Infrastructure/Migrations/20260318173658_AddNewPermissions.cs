using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Shopizy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNewPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("759b8d6d-ffda-4c99-bf29-ed335c029a5c"), "modify:wishlist" },
                    { new Guid("9b259d3d-b634-4232-9deb-e5fdb20d7a64"), "get:wishlist" },
                    { new Guid("b25fd8ef-723d-47d1-8b31-648a69733975"), "get:dashboard" },
                    { new Guid("d99cab25-5af2-4b9c-9fad-385e4715d7f2"), "delete:wishlist" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("759b8d6d-ffda-4c99-bf29-ed335c029a5c"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("9b259d3d-b634-4232-9deb-e5fdb20d7a64"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("b25fd8ef-723d-47d1-8b31-648a69733975"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("d99cab25-5af2-4b9c-9fad-385e4715d7f2"));
        }
    }
}
