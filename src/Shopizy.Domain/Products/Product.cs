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
    public DateTime CreatedOn { get; private set; }
    public DateTime? ModifiedOn { get; private set; }

    public IReadOnlyList<ProductImage> ProductImages => _productImages.AsReadOnly();
    public IReadOnlyList<ProductReview> ProductReviews => _productReviews.AsReadOnly();

    public static Product Create(
        string name,
        string description,
        CategoryId categoryId,
        string sku,
        Price unitPrice,
        decimal? discount,
        string brand,
        string barcode,
        string tags
    )
    {
        return new Product(
            ProductId.CreateUnique(),
            name,
            description,
            categoryId,
            sku,
            0,
            unitPrice,
            discount,
            brand,
            barcode,
            tags,
            AverageRating.CreateNew(0)
        );
    }

    public void Update(
        string name,
        string description,
        CategoryId categoryId,
        string sku,
        Price unitPrice,
        decimal? discount,
        string brand,
        string barcode,
        string tags
    )
    {
        Name = name;
        Description = description;
        CategoryId = categoryId;
        SKU = sku;
        UnitPrice = unitPrice;
        Discount = discount;
        Brand = brand;
        Barcode = barcode;
        Tags = tags;
        ModifiedOn = DateTime.UtcNow;
    }

    public void AddProductImages(List<ProductImage> productImages)
    {
        _productImages.AddRange(productImages);
    }

    public void AddProductImage(ProductImage productImage)
    {
        _productImages.Add(productImage);
    }

    public void RemoveProductImage(ProductImage productImage)
    {
        _ = _productImages.Remove(productImage);
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
        AverageRating averageRating
    )
        : base(productId)
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
        CreatedOn = DateTime.UtcNow;
    }

    private Product() { }
}
