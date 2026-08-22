using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Shopizy.Infrastructure.Common.Persistence;

#nullable disable

namespace Shopizy.Infrastructure.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(AppDbContext))]
    [Migration("20260822150000_AddAdvancedPromoCodeFields")]
    public partial class AddAdvancedPromoCodeFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PromoType",
                table: "PromoCodes",
                type: "int",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder.AddColumn<decimal>(
                name: "MinimumOrderAmount",
                table: "PromoCodes",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true
            );

            migrationBuilder.AddColumn<decimal>(
                name: "MaxDiscountAmount",
                table: "PromoCodes",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true
            );

            migrationBuilder.AddColumn<Guid>(
                name: "TargetCategoryId",
                table: "PromoCodes",
                type: "uniqueidentifier",
                nullable: true
            );

            migrationBuilder.AddColumn<int>(
                name: "BuyQuantity",
                table: "PromoCodes",
                type: "int",
                nullable: true
            );

            migrationBuilder.AddColumn<int>(
                name: "GetQuantity",
                table: "PromoCodes",
                type: "int",
                nullable: true
            );

            migrationBuilder.AddColumn<decimal>(
                name: "GetDiscountPercentage",
                table: "PromoCodes",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true
            );

            migrationBuilder.AddColumn<int>(
                name: "UsageLimit",
                table: "PromoCodes",
                type: "int",
                nullable: true
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "PromoCodes",
                type: "smalldatetime",
                nullable: true
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "PromoCodes",
                type: "smalldatetime",
                nullable: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "PromoType", table: "PromoCodes");
            migrationBuilder.DropColumn(name: "MinimumOrderAmount", table: "PromoCodes");
            migrationBuilder.DropColumn(name: "MaxDiscountAmount", table: "PromoCodes");
            migrationBuilder.DropColumn(name: "TargetCategoryId", table: "PromoCodes");
            migrationBuilder.DropColumn(name: "BuyQuantity", table: "PromoCodes");
            migrationBuilder.DropColumn(name: "GetQuantity", table: "PromoCodes");
            migrationBuilder.DropColumn(name: "GetDiscountPercentage", table: "PromoCodes");
            migrationBuilder.DropColumn(name: "UsageLimit", table: "PromoCodes");
            migrationBuilder.DropColumn(name: "StartDate", table: "PromoCodes");
            migrationBuilder.DropColumn(name: "EndDate", table: "PromoCodes");
        }
    }
}
