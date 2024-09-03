using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace shopizy.Infrastructure.Migrations;

/// <inheritdoc />
public partial class UpdateAddress : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "Address_Line",
            table: "Users",
            newName: "Address_Street");

        migrationBuilder.RenameColumn(
            name: "BillingAddress_Line",
            table: "Payments",
            newName: "BillingAddress_Street");

        migrationBuilder.RenameColumn(
            name: "ShippingAddress_Line",
            table: "Orders",
            newName: "ShippingAddress_Street");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "Address_Street",
            table: "Users",
            newName: "Address_Line");

        migrationBuilder.RenameColumn(
            name: "BillingAddress_Street",
            table: "Payments",
            newName: "BillingAddress_Line");

        migrationBuilder.RenameColumn(
            name: "ShippingAddress_Street",
            table: "Orders",
            newName: "ShippingAddress_Line");
    }
}
