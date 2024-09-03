using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace shopizy.Infrastructure.Migrations;

/// <inheritdoc />
public partial class InitialChanges : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Categories",
            columns: table =>
                new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
            constraints: table =>
            {
                table.PrimaryKey("PK_Categories", x => x.Id);
            }
        );

        migrationBuilder.CreateTable(
            name: "PromoCodes",
            columns: table =>
                new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(
                        type: "nvarchar(15)",
                        maxLength: 15,
                        nullable: false
                    ),
                    Description = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: true
                    ),
                    Discount = table.Column<decimal>(
                        type: "decimal(18,2)",
                        precision: 18,
                        scale: 2,
                        nullable: false
                    ),
                    IsPerchantage = table.Column<bool>(
                        type: "bit",
                        nullable: false,
                        defaultValue: true
                    ),
                    IsActive = table.Column<bool>(
                        type: "bit",
                        nullable: false,
                        defaultValue: true
                    ),
                    NumOfTimeUsed = table.Column<int>(
                        type: "int",
                        nullable: false,
                        defaultValue: 0
                    ),
                    CreatedOn = table.Column<DateTime>(type: "smalldatetime", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "smalldatetime", nullable: false)
                },
            constraints: table =>
            {
                table.PrimaryKey("PK_PromoCodes", x => x.Id);
            }
        );

        migrationBuilder.CreateTable(
            name: "Users",
            columns: table =>
                new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(
                        type: "nvarchar(50)",
                        maxLength: 50,
                        nullable: false
                    ),
                    LastName = table.Column<string>(
                        type: "nvarchar(50)",
                        maxLength: 50,
                        nullable: false
                    ),
                    Phone = table.Column<string>(
                        type: "nvarchar(15)",
                        maxLength: 15,
                        nullable: false
                    ),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "smalldatetime", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "smalldatetime", nullable: false)
                },
            constraints: table =>
            {
                table.PrimaryKey("PK_Users", x => x.Id);
            }
        );

        migrationBuilder.CreateTable(
            name: "Products",
            columns: table =>
                new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    Description = table.Column<string>(
                        type: "nvarchar(200)",
                        maxLength: 200,
                        nullable: false
                    ),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SKU = table.Column<string>(
                        type: "nvarchar(50)",
                        maxLength: 50,
                        nullable: false
                    ),
                    StockQuantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice_Amount = table.Column<decimal>(
                        type: "decimal(18,2)",
                        precision: 18,
                        scale: 2,
                        nullable: false
                    ),
                    UnitPrice_Currency = table.Column<int>(type: "int", nullable: false),
                    Discount = table.Column<decimal>(
                        type: "decimal(18,2)",
                        precision: 18,
                        scale: 2,
                        nullable: true
                    ),
                    Brand = table.Column<string>(
                        type: "nvarchar(50)",
                        maxLength: 50,
                        nullable: true
                    ),
                    Barcode = table.Column<string>(
                        type: "nvarchar(50)",
                        maxLength: 50,
                        nullable: true
                    ),
                    Tags = table.Column<string>(
                        type: "nvarchar(200)",
                        maxLength: 200,
                        nullable: true
                    ),
                    AverageRating_Value = table.Column<double>(
                        type: "float(18)",
                        precision: 18,
                        scale: 2,
                        nullable: false
                    ),
                    AverageRating_NumRatings = table.Column<int>(type: "int", nullable: false),
                    BreadCrums = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: true
                    ),
                    CreatedOn = table.Column<DateTime>(type: "smalldatetime", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "smalldatetime", nullable: false)
                },
            constraints: table =>
            {
                table.PrimaryKey("PK_Products", x => x.Id);
                table.ForeignKey(
                    name: "FK_Products_Categories_CategoryId",
                    column: x => x.CategoryId,
                    principalTable: "Categories",
                    principalColumn: "Id"
                );
            }
        );

        migrationBuilder.CreateTable(
            name: "Customers",
            columns: table =>
                new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProfileImageUrl = table.Column<string>(
                        type: "nvarchar(max)",
                        nullable: true
                    ),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Address_Line = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: true
                    ),
                    Address_City = table.Column<string>(
                        type: "nvarchar(30)",
                        maxLength: 30,
                        nullable: true
                    ),
                    Address_State = table.Column<string>(
                        type: "nvarchar(30)",
                        maxLength: 30,
                        nullable: true
                    ),
                    Address_Country = table.Column<string>(
                        type: "nvarchar(30)",
                        maxLength: 30,
                        nullable: true
                    ),
                    Address_ZipCode = table.Column<string>(
                        type: "nvarchar(10)",
                        maxLength: 10,
                        nullable: true
                    ),
                    CreatedOn = table.Column<DateTime>(type: "smalldatetime", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "smalldatetime", nullable: false)
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

        migrationBuilder.CreateTable(
            name: "ProductImages",
            columns: table =>
                new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Seq = table.Column<int>(type: "int", nullable: false)
                },
            constraints: table =>
            {
                table.PrimaryKey("PK_ProductImages", x => new { x.ProductId, x.Id });
                table.ForeignKey(
                    name: "FK_ProductImages_Products_ProductId",
                    column: x => x.ProductId,
                    principalTable: "Products",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade
                );
            }
        );

        migrationBuilder.CreateTable(
            name: "Orders",
            columns: table =>
                new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeliveryCharge_Amount = table.Column<decimal>(
                        type: "decimal(18,2)",
                        precision: 18,
                        scale: 2,
                        nullable: false
                    ),
                    DeliveryCharge_Currency = table.Column<int>(type: "int", nullable: false),
                    OrderStatus = table.Column<int>(type: "int", nullable: false),
                    PromoCode = table.Column<string>(
                        type: "nvarchar(15)",
                        maxLength: 15,
                        nullable: true
                    ),
                    ShippingAddress_Line = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    ShippingAddress_City = table.Column<string>(
                        type: "nvarchar(30)",
                        maxLength: 30,
                        nullable: false
                    ),
                    ShippingAddress_State = table.Column<string>(
                        type: "nvarchar(30)",
                        maxLength: 30,
                        nullable: false
                    ),
                    ShippingAddress_Country = table.Column<string>(
                        type: "nvarchar(30)",
                        maxLength: 30,
                        nullable: false
                    ),
                    ShippingAddress_ZipCode = table.Column<string>(
                        type: "nvarchar(10)",
                        maxLength: 10,
                        nullable: false
                    ),
                    PaymentStatus = table.Column<string>(
                        type: "nvarchar(20)",
                        maxLength: 20,
                        nullable: false
                    ),
                    CreatedOn = table.Column<DateTime>(type: "smalldatetime", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "smalldatetime", nullable: false)
                },
            constraints: table =>
            {
                table.PrimaryKey("PK_Orders", x => x.Id);
                table.ForeignKey(
                    name: "FK_Orders_Customers_CustomerId",
                    column: x => x.CustomerId,
                    principalTable: "Customers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade
                );
            }
        );

        migrationBuilder.CreateTable(
            name: "ProductReviews",
            columns: table =>
                new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Rating_Value = table.Column<double>(
                        type: "float(18)",
                        precision: 18,
                        scale: 2,
                        nullable: false
                    ),
                    Comment = table.Column<string>(
                        type: "nvarchar(1000)",
                        maxLength: 1000,
                        nullable: true
                    ),
                    CreatedOn = table.Column<DateTime>(type: "smalldatetime", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "smalldatetime", nullable: false)
                },
            constraints: table =>
            {
                table.PrimaryKey("PK_ProductReviews", x => x.Id);
                table.ForeignKey(
                    name: "FK_ProductReviews_Customers_CustomerId",
                    column: x => x.CustomerId,
                    principalTable: "Customers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade
                );
                table.ForeignKey(
                    name: "FK_ProductReviews_Products_ProductId",
                    column: x => x.ProductId,
                    principalTable: "Products",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade
                );
            }
        );

        migrationBuilder.CreateTable(
            name: "OrderItems",
            columns: table =>
                new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    PictureUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UnitPrice_Amount = table.Column<decimal>(
                        type: "decimal(18,2)",
                        precision: 18,
                        scale: 2,
                        nullable: false
                    ),
                    UnitPrice_Currency = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Discount = table.Column<decimal>(
                        type: "decimal(18,2)",
                        precision: 18,
                        scale: 2,
                        nullable: false
                    )
                },
            constraints: table =>
            {
                table.PrimaryKey("PK_OrderItems", x => new { x.Id, x.OrderId });
                table.ForeignKey(
                    name: "FK_OrderItems_Orders_OrderId",
                    column: x => x.OrderId,
                    principalTable: "Orders",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade
                );
            }
        );

        migrationBuilder.CreateTable(
            name: "Payments",
            columns: table =>
                new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentMethod = table.Column<string>(
                        type: "nvarchar(20)",
                        maxLength: 20,
                        nullable: false
                    ),
                    TransactionId = table.Column<string>(
                        type: "nvarchar(50)",
                        maxLength: 50,
                        nullable: true
                    ),
                    PaymentStatus = table.Column<int>(type: "int", nullable: false),
                    Total_Amount = table.Column<decimal>(
                        type: "decimal(18,2)",
                        precision: 18,
                        scale: 2,
                        nullable: false
                    ),
                    Total_Currency = table.Column<int>(type: "int", nullable: false),
                    BillingAddress_Line = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    BillingAddress_City = table.Column<string>(
                        type: "nvarchar(30)",
                        maxLength: 30,
                        nullable: false
                    ),
                    BillingAddress_State = table.Column<string>(
                        type: "nvarchar(30)",
                        maxLength: 30,
                        nullable: false
                    ),
                    BillingAddress_Country = table.Column<string>(
                        type: "nvarchar(30)",
                        maxLength: 30,
                        nullable: false
                    ),
                    BillingAddress_ZipCode = table.Column<string>(
                        type: "nvarchar(10)",
                        maxLength: 10,
                        nullable: false
                    ),
                    CreatedOn = table.Column<DateTime>(type: "smalldatetime", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "smalldatetime", nullable: false)
                },
            constraints: table =>
            {
                table.PrimaryKey("PK_Payments", x => x.Id);
                table.ForeignKey(
                    name: "FK_Payments_Customers_CustomerId",
                    column: x => x.CustomerId,
                    principalTable: "Customers",
                    principalColumn: "Id"
                );
                table.ForeignKey(
                    name: "FK_Payments_Orders_OrderId",
                    column: x => x.OrderId,
                    principalTable: "Orders",
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

        migrationBuilder.CreateIndex(
            name: "IX_OrderItems_OrderId",
            table: "OrderItems",
            column: "OrderId"
        );

        migrationBuilder.CreateIndex(
            name: "IX_Orders_CustomerId",
            table: "Orders",
            column: "CustomerId"
        );

        migrationBuilder.CreateIndex(
            name: "IX_Payments_CustomerId",
            table: "Payments",
            column: "CustomerId",
            unique: true
        );

        migrationBuilder.CreateIndex(
            name: "IX_Payments_OrderId",
            table: "Payments",
            column: "OrderId",
            unique: true
        );

        migrationBuilder.CreateIndex(
            name: "IX_ProductReviews_CustomerId",
            table: "ProductReviews",
            column: "CustomerId"
        );

        migrationBuilder.CreateIndex(
            name: "IX_ProductReviews_ProductId",
            table: "ProductReviews",
            column: "ProductId"
        );

        migrationBuilder.CreateIndex(
            name: "IX_Products_CategoryId",
            table: "Products",
            column: "CategoryId"
        );

        migrationBuilder.CreateIndex(
            name: "IX_PromoCodes_Code",
            table: "PromoCodes",
            column: "Code"
        );

        migrationBuilder.CreateIndex(name: "IX_Users_Phone", table: "Users", column: "Phone");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "OrderItems");

        migrationBuilder.DropTable(name: "Payments");

        migrationBuilder.DropTable(name: "ProductImages");

        migrationBuilder.DropTable(name: "ProductReviews");

        migrationBuilder.DropTable(name: "PromoCodes");

        migrationBuilder.DropTable(name: "Orders");

        migrationBuilder.DropTable(name: "Products");

        migrationBuilder.DropTable(name: "Customers");

        migrationBuilder.DropTable(name: "Categories");

        migrationBuilder.DropTable(name: "Users");
    }
}
