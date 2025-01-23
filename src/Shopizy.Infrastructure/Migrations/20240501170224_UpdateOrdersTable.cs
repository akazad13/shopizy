using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace shopizy.Infrastructure.Migrations;

/// <inheritdoc />
public partial class UpdateOrdersTable : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "CancellationReason",
            table: "Orders",
            type: "nvarchar(200)",
            maxLength: 200,
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "CancellationReason",
            table: "Orders");
    }
}
