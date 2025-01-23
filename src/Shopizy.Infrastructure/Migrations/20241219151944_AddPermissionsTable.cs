using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace shopizy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPermissionsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(
                        type: "nvarchar(50)",
                        maxLength: 50,
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "UserPermissionIds",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PermissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPermissionIds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPermissionIds_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissionIds_UserId",
                table: "UserPermissionIds",
                column: "UserId"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Permissions");

            migrationBuilder.DropTable(name: "UserPermissionIds");
        }
    }
}
