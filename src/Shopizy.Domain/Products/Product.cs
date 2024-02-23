using Shopizy.Domain.Common.Models;
using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.Products.Entities;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Domain.Products;

public sealed class Product : AggregateRoot<ProductId, Guid>
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string Category { get; private set; }
    public string SKU { get; private set; }
    public int StockQuantity { get; private set; }
    public Price UnitPrice { get; private set; }
    public float Discount { get; private set; }
    public string Brand { get; private set; }
    public string Barcode { get; private set; }
    public string Tags { get; private set; }
    public AverageRating AverageRating { get; private set; }
    public string BreadCrums { get; private set; }
    public DateTime CreatedDateTime { get; private set; }
    public DateTime UpdatedDateTime { get; private set; }

    private readonly List<ProductImage> _productImages = [];
    public IReadOnlyList<ProductImage> Reservations => _productImages.AsReadOnly();

    public static Product Create(
        string name,
        string description,
        string category,
        string sku,
        int stockQuantity,
        Price unitPrice,
        float discount,
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
            category,
            sku,
            stockQuantity,
            unitPrice,
            discount,
            brand,
            barcode,
            tags,
            AverageRating.CreateNew(0),
            breadCrums,
            DateTime.Now,
            DateTime.Now,
            productImages ?? []
        );
    }

    private Product(
        ProductId id,
        string name,
        string description,
        string category,
        string sku,
        int stockQuantity,
        Price unitPrice,
        float discount,
        string brand,
        string barcode,
        string tags,
        AverageRating averageRating,
        string breadCrums,
        DateTime createdDateTime,
        DateTime updatedDateTime,
        List<ProductImage> productImages
    ) : base(id)
    {
        Name = name;
        Description = description;
        Category = category;
        SKU = sku;
        StockQuantity = stockQuantity;
        UnitPrice = unitPrice;
        Discount = discount;
        Brand = brand;
        Barcode = barcode;
        Tags = tags;
        AverageRating = averageRating;
        BreadCrums = breadCrums;
        CreatedDateTime = createdDateTime;
        UpdatedDateTime = updatedDateTime;
        _productImages = productImages;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Product() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
