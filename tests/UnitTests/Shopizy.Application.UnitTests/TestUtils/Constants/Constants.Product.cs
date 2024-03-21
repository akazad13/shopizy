using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Application.UnitTests.TestUtils.Constants;

public static partial class Constants
{
    public static class Product
    {
        public static readonly ProductId Id = ProductId.Create(Guid.NewGuid());
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
    }
}
