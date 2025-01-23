using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace shopizy.Infrastructure.Migrations;

/// <inheritdoc />
public partial class UpdateTables : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Orders_Customers_CustomerId",
            table: "Orders");

        migrationBuilder.DropForeignKey(
            name: "FK_Payments_Customers_CustomerId",
            table: "Payments");

        migrationBuilder.DropForeignKey(
            name: "FK_ProductReviews_Customers_CustomerId",
            table: "ProductReviews");

        migrationBuilder.RenameColumn(
            name: "CustomerId",
            table: "ProductReviews",
            newName: "UserId");

        migrationBuilder.RenameIndex(
            name: "IX_ProductReviews_CustomerId",
            table: "ProductReviews",
            newName: "IX_ProductReviews_UserId");

        migrationBuilder.RenameColumn(
            name: "CustomerId",
            table: "Payments",
            newName: "UserId");

        migrationBuilder.RenameIndex(
            name: "IX_Payments_CustomerId",
            table: "Payments",
            newName: "IX_Payments_UserId");

        migrationBuilder.RenameColumn(
            name: "CustomerId",
            table: "Orders",
            newName: "UserId");

        migrationBuilder.RenameIndex(
            name: "IX_Orders_CustomerId",
            table: "Orders",
            newName: "IX_Orders_UserId");

        migrationBuilder.RenameColumn(
            name: "CustomerId",
            table: "Carts",
            newName: "UserId");

        migrationBuilder.RenameIndex(
            name: "IX_Carts_CustomerId",
            table: "Carts",
            newName: "IX_Carts_UserId");

        migrationBuilder.AddForeignKey(
            name: "FK_Orders_Users_UserId",
            table: "Orders",
            column: "UserId",
            principalTable: "Users",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_Payments_Users_UserId",
            table: "Payments",
            column: "UserId",
            principalTable: "Users",
            principalColumn: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_ProductReviews_Users_UserId",
            table: "ProductReviews",
            column: "UserId",
            principalTable: "Users",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Orders_Users_UserId",
            table: "Orders");

        migrationBuilder.DropForeignKey(
            name: "FK_Payments_Users_UserId",
            table: "Payments");

        migrationBuilder.DropForeignKey(
            name: "FK_ProductReviews_Users_UserId",
            table: "ProductReviews");

        migrationBuilder.RenameColumn(
            name: "UserId",
            table: "ProductReviews",
            newName: "CustomerId");

        migrationBuilder.RenameIndex(
            name: "IX_ProductReviews_UserId",
            table: "ProductReviews",
            newName: "IX_ProductReviews_CustomerId");

        migrationBuilder.RenameColumn(
            name: "UserId",
            table: "Payments",
            newName: "CustomerId");

        migrationBuilder.RenameIndex(
            name: "IX_Payments_UserId",
            table: "Payments",
            newName: "IX_Payments_CustomerId");

        migrationBuilder.RenameColumn(
            name: "UserId",
            table: "Orders",
            newName: "CustomerId");

        migrationBuilder.RenameIndex(
            name: "IX_Orders_UserId",
            table: "Orders",
            newName: "IX_Orders_CustomerId");

        migrationBuilder.RenameColumn(
            name: "UserId",
            table: "Carts",
            newName: "CustomerId");

        migrationBuilder.RenameIndex(
            name: "IX_Carts_UserId",
            table: "Carts",
            newName: "IX_Carts_CustomerId");

        migrationBuilder.AddForeignKey(
            name: "FK_Orders_Customers_CustomerId",
            table: "Orders",
            column: "CustomerId",
            principalTable: "Customers",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_Payments_Customers_CustomerId",
            table: "Payments",
            column: "CustomerId",
            principalTable: "Customers",
            principalColumn: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_ProductReviews_Customers_CustomerId",
            table: "ProductReviews",
            column: "CustomerId",
            principalTable: "Customers",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }
}
