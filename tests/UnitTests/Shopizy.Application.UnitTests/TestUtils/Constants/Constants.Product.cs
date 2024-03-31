using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Application.UnitTests.TestUtils.Constants;

public static partial class Constants
{
    public static class Product
    {
        public static readonly ProductId Id = ProductId.Create(
            new Guid("dd0aa32a-f7ab-4d48-b33e-1a3c1092f1e2")
        );
        public const string Name = "Product Name";
        public const string Description = "Product Description";
        public const decimal UnitPrice = 100;
        public const int Currency = 0;
        public const int Discount = 30;
        public const string Sku = "Product SKU";
        public const string Brand = "Product Brand Name";
        public const string Tags = "Product Tag";
        public const string Barcode = "Product Barcode";
        public const int StockQuantity = 50;
        public static readonly Price Price = Price.CreateNew(100, 0);
    }

    public static class ProductImage
    {
        public static readonly ProductImageId Id = ProductImageId.Create(
            new Guid("dd0aa32a-f7ab-4d48-b33e-1a3c1092f1e4")
        );
        public const string ImageUrl = "https://res.cloudinary.com/test/image/upload/test";
        public const int Seq = 1;
        public const string PublicId = "publicId";
    }
}
