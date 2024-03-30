using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace shopizy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedCartTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Carts",
                columns: table =>
                    new
                    {
                        Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                        CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                        CreatedOn = table.Column<DateTime>(type: "smalldatetime", nullable: false),
                        ModifiedOn = table.Column<DateTime>(type: "smalldatetime", nullable: false)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carts", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "LineItems",
                columns: table =>
                    new
                    {
                        Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                        CartId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                        ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                        Quantity = table.Column<int>(type: "int", nullable: false)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LineItems", x => new { x.Id, x.CartId });
                    table.ForeignKey(
                        name: "FK_LineItems_Carts_CartId",
                        column: x => x.CartId,
                        principalTable: "Carts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_LineItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_Carts_CustomerId",
                table: "Carts",
                column: "CustomerId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_LineItems_CartId",
                table: "LineItems",
                column: "CartId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_LineItems_ProductId",
                table: "LineItems",
                column: "ProductId"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "LineItems");

            migrationBuilder.DropTable(name: "Carts");
        }
    }
}
