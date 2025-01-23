using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace shopizy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addEmailColumnsToUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: ""
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Email", table: "Users");
        }
    }
}
