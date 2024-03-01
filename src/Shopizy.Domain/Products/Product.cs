using Shopizy.Domain.Categories.ValueObjects;
using Shopizy.Domain.Common.Models;
using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.ProductReviews;
using Shopizy.Domain.Products.Entities;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Domain.Products;

public sealed class Product : AggregateRoot<ProductId, Guid>
{
    private readonly List<ProductImage> _productImages = [];
    private readonly List<ProductReview> _productReviews = [];
    public string Name { get; private set; }
    public string Description { get; private set; }
    public CategoryId CategoryId { get; private set; }
    public string SKU { get; private set; }
    public int StockQuantity { get; private set; }
    public Price UnitPrice { get; private set; }
    public decimal? Discount { get; private set; }
    public string Brand { get; private set; }
    public string Barcode { get; private set; }
    public string Tags { get; private set; }
    public AverageRating AverageRating { get; private set; }
    public string BreadCrums { get; private set; }
    public DateTime CreatedOn { get; private set; }
    public DateTime ModifiedOn { get; private set; }

    public IReadOnlyList<ProductImage> ProductImages => _productImages.AsReadOnly();
    public IReadOnlyList<ProductReview> ProductReviews => _productReviews.AsReadOnly();

    public static Product Create(
        string name,
        string description,
        CategoryId categoryId,
        string sku,
        int stockQuantity,
        Price unitPrice,
        decimal? discount,
        string brand,
        string barcode,
        string tags,
        string breadCrums,
        List<ProductImage>? productImages = null
    )
    {
        return new Product(
            ProductId.CreateUnique(),
            name,
            description,
            categoryId,
            sku,
            stockQuantity,
            unitPrice,
            discount,
            brand,
            barcode,
            tags,
            AverageRating.CreateNew(0),
            breadCrums,
            DateTime.UtcNow,
            DateTime.UtcNow,
            productImages ?? []
        );
    }

    private Product(
        ProductId productId,
        string name,
        string description,
        CategoryId categoryId,
        string sku,
        int stockQuantity,
        Price unitPrice,
        decimal? discount,
        string brand,
        string barcode,
        string tags,
        AverageRating averageRating,
        string breadCrums,
        DateTime createdOn,
        DateTime modifiedOn,
        List<ProductImage> productImages
    ) : base(productId)
    {
        Name = name;
        Description = description;
        CategoryId = categoryId;
        SKU = sku;
        StockQuantity = stockQuantity;
        UnitPrice = unitPrice;
        Discount = discount;
        Brand = brand;
        Barcode = barcode;
        Tags = tags;
        AverageRating = averageRating;
        BreadCrums = breadCrums;
        CreatedOn = createdOn;
        ModifiedOn = modifiedOn;
        _productImages = productImages;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Product() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
