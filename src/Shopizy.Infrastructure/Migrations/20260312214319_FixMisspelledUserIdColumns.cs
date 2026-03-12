using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shopizy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixMisspelledUserIdColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderIds_Users_UseId",
                table: "OrderIds");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductReviewIds_Users_UseId",
                table: "ProductReviewIds");

            migrationBuilder.RenameColumn(
                name: "UseId",
                table: "ProductReviewIds",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductReviewIds_UseId",
                table: "ProductReviewIds",
                newName: "IX_ProductReviewIds_UserId");

            migrationBuilder.RenameColumn(
                name: "UseId",
                table: "OrderIds",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderIds_UseId",
                table: "OrderIds",
                newName: "IX_OrderIds_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderIds_Users_UserId",
                table: "OrderIds",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductReviewIds_Users_UserId",
                table: "ProductReviewIds",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderIds_Users_UserId",
                table: "OrderIds");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductReviewIds_Users_UserId",
                table: "ProductReviewIds");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "ProductReviewIds",
                newName: "UseId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductReviewIds_UserId",
                table: "ProductReviewIds",
                newName: "IX_ProductReviewIds_UseId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "OrderIds",
                newName: "UseId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderIds_UserId",
                table: "OrderIds",
                newName: "IX_OrderIds_UseId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderIds_Users_UseId",
                table: "OrderIds",
                column: "UseId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductReviewIds_Users_UseId",
                table: "ProductReviewIds",
                column: "UseId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
