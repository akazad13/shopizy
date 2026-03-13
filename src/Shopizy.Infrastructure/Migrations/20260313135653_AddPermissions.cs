using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Shopizy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class remaining : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("0374e597-604e-4146-8f40-8c994d26c290"), "get:user" },
                    { new Guid("0529a2f2-7507-4fa5-9daf-68829f9d7fc4"), "create:user" },
                    { new Guid("0c65a58a-d472-4d5d-848e-eac46f988f5d"), "get:product" },
                    { new Guid("1679ba61-9b46-457e-9974-f02300b9a1d5"), "delete:product" },
                    { new Guid("1dd03229-b8da-4926-8e4a-12b27a0ff5e7"), "create:product" },
                    { new Guid("20082930-3857-4b34-80d0-e256b9b585d8"), "modify:cart" },
                    { new Guid("249e733d-5bdc-49c3-91ca-06ae25a9c897"), "create:cart" },
                    { new Guid("2a19090a-b3f3-4b30-9ced-934ee0503d26"), "create:order" },
                    { new Guid("43b3188d-6e85-479a-9fd7-0186fca97f52"), "modify:product" },
                    { new Guid("4b88cb16-0228-4669-ba7f-b75f42a3b7af"), "get:cart" },
                    { new Guid("5e2a486b-d9a0-4f83-8ff2-c56ef97ce485"), "get:category" },
                    { new Guid("626da392-0bbf-4c3f-8909-a8fc18f4dc43"), "modify:category" },
                    { new Guid("6811001e-28ae-4bb1-bbb8-99be33a21302"), "delete:category" },
                    { new Guid("80366e1a-634d-4579-9245-164166e1146b"), "delete:user" },
                    { new Guid("9601ba5e-eb54-4487-bfe0-563462d3cc25"), "get:order" },
                    { new Guid("acd9d507-ac45-4cd2-b0f4-91126c71319a"), "modify:order" },
                    { new Guid("c920a577-1669-4167-b056-5e0a03329c55"), "modify:user" },
                    { new Guid("d6c2e3c6-314b-4f2e-a407-34139b145771"), "delete:cart" },
                    { new Guid("dd25381d-063c-4a3a-9539-deec640919a4"), "delete:order" },
                    { new Guid("f49bbc15-aa8b-4752-af66-e3e00afc173d"), "create:category" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0374e597-604e-4146-8f40-8c994d26c290"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0529a2f2-7507-4fa5-9daf-68829f9d7fc4"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0c65a58a-d472-4d5d-848e-eac46f988f5d"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("1679ba61-9b46-457e-9974-f02300b9a1d5"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("1dd03229-b8da-4926-8e4a-12b27a0ff5e7"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("20082930-3857-4b34-80d0-e256b9b585d8"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("249e733d-5bdc-49c3-91ca-06ae25a9c897"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("2a19090a-b3f3-4b30-9ced-934ee0503d26"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("43b3188d-6e85-479a-9fd7-0186fca97f52"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("4b88cb16-0228-4669-ba7f-b75f42a3b7af"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("5e2a486b-d9a0-4f83-8ff2-c56ef97ce485"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("626da392-0bbf-4c3f-8909-a8fc18f4dc43"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("6811001e-28ae-4bb1-bbb8-99be33a21302"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("80366e1a-634d-4579-9245-164166e1146b"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("9601ba5e-eb54-4487-bfe0-563462d3cc25"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("acd9d507-ac45-4cd2-b0f4-91126c71319a"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("c920a577-1669-4167-b056-5e0a03329c55"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("d6c2e3c6-314b-4f2e-a407-34139b145771"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("dd25381d-063c-4a3a-9539-deec640919a4"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("f49bbc15-aa8b-4752-af66-e3e00afc173d"));
        }
    }
}
