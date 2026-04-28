using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shopizy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOptimisticConcurrency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Orders",
                type: "rowversion",
                rowVersion: true,
                nullable: true
            );

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Carts",
                type: "rowversion",
                rowVersion: true,
                nullable: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "RowVersion", table: "Orders");

            migrationBuilder.DropColumn(name: "RowVersion", table: "Carts");
        }
    }
}
