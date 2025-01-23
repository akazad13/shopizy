using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace shopizy.Infrastructure.Migrations;

/// <inheritdoc />
public partial class UpdateProductImageTable : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "PublicId",
            table: "ProductImages",
            type: "nvarchar(100)",
            maxLength: 100,
            nullable: false,
            defaultValue: "");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "PublicId",
            table: "ProductImages");
    }
}
