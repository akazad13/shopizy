using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shopizy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLoyaltyPointsToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "LoyaltyDiscountApplied",
                table: "Orders",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m
            );

            migrationBuilder.AddColumn<int>(
                name: "LoyaltyPointsRedeemed",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "LoyaltyDiscountApplied", table: "Orders");

            migrationBuilder.DropColumn(name: "LoyaltyPointsRedeemed", table: "Orders");
        }
    }
}
