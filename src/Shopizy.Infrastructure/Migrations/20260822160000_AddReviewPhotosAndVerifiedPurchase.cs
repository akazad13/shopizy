using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Shopizy.Infrastructure.Common.Persistence;

#nullable disable

namespace Shopizy.Infrastructure.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(AppDbContext))]
    [Migration("20260822160000_AddReviewPhotosAndVerifiedPurchase")]
    public partial class AddReviewPhotosAndVerifiedPurchase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Headline",
                table: "ProductReviews",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true
            );

            migrationBuilder.AddColumn<bool>(
                name: "IsVerifiedPurchase",
                table: "ProductReviews",
                type: "bit",
                nullable: false,
                defaultValue: false
            );

            migrationBuilder.AddColumn<int>(
                name: "HelpfulVotesCount",
                table: "ProductReviews",
                type: "int",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder.AddColumn<string>(
                name: "ImageUrls",
                table: "ProductReviews",
                type: "nvarchar(max)",
                nullable: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Headline", table: "ProductReviews");
            migrationBuilder.DropColumn(name: "IsVerifiedPurchase", table: "ProductReviews");
            migrationBuilder.DropColumn(name: "HelpfulVotesCount", table: "ProductReviews");
            migrationBuilder.DropColumn(name: "ImageUrls", table: "ProductReviews");
        }
    }
}
