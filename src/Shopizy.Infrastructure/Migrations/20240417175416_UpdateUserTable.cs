using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace shopizy.Infrastructure.Migrations;

/// <inheritdoc />
public partial class UpdateUserTable : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "Customers");

        migrationBuilder.AddColumn<string>(
            name: "Address_City",
            table: "Users",
            type: "nvarchar(30)",
            maxLength: 30,
            nullable: true
        );

        migrationBuilder.AddColumn<string>(
            name: "Address_Country",
            table: "Users",
            type: "nvarchar(30)",
            maxLength: 30,
            nullable: true
        );

        migrationBuilder.AddColumn<string>(
            name: "Address_Line",
            table: "Users",
            type: "nvarchar(100)",
            maxLength: 100,
            nullable: true
        );

        migrationBuilder.AddColumn<string>(
            name: "Address_State",
            table: "Users",
            type: "nvarchar(30)",
            maxLength: 30,
            nullable: true
        );

        migrationBuilder.AddColumn<string>(
            name: "Address_ZipCode",
            table: "Users",
            type: "nvarchar(10)",
            maxLength: 10,
            nullable: true
        );

        migrationBuilder.AddColumn<string>(
            name: "ProfileImageUrl",
            table: "Users",
            type: "nvarchar(max)",
            nullable: true
        );
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name: "Address_City", table: "Users");

        migrationBuilder.DropColumn(name: "Address_Country", table: "Users");

        migrationBuilder.DropColumn(name: "Address_Line", table: "Users");

        migrationBuilder.DropColumn(name: "Address_State", table: "Users");

        migrationBuilder.DropColumn(name: "Address_ZipCode", table: "Users");

        migrationBuilder.DropColumn(name: "ProfileImageUrl", table: "Users");

        migrationBuilder.CreateTable(
            name: "Customers",
            columns: table =>
                new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "smalldatetime", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "smalldatetime", nullable: false),
                    ProfileImageUrl = table.Column<string>(
                        type: "nvarchar(max)",
                        nullable: true
                    ),
                    Address_City = table.Column<string>(
                        type: "nvarchar(30)",
                        maxLength: 30,
                        nullable: true
                    ),
                    Address_Country = table.Column<string>(
                        type: "nvarchar(30)",
                        maxLength: 30,
                        nullable: true
                    ),
                    Address_Line = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: true
                    ),
                    Address_State = table.Column<string>(
                        type: "nvarchar(30)",
                        maxLength: 30,
                        nullable: true
                    ),
                    Address_ZipCode = table.Column<string>(
                        type: "nvarchar(10)",
                        maxLength: 10,
                        nullable: true
                    )
                },
            constraints: table =>
            {
                table.PrimaryKey("PK_Customers", x => x.Id);
                table.ForeignKey(
                    name: "FK_Customers_Users_UserId",
                    column: x => x.UserId,
                    principalTable: "Users",
                    principalColumn: "Id"
                );
            }
        );

        migrationBuilder.CreateIndex(
            name: "IX_Customers_UserId",
            table: "Customers",
            column: "UserId",
            unique: true
        );
    }
}
