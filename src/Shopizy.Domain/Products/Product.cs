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
    public string ShortDescription { get; private set; }
    public string Description { get; private set; }
    public CategoryId CategoryId { get; private set; }
    public string SKU { get; private set; }
    public int StockQuantity { get; private set; }
    public Price UnitPrice { get; private set; }
    public decimal? Discount { get; private set; }
    public string Brand { get; private set; }
    public string Colors { get; private set; }
    public string Sizes { get; private set; }
    public int Favourites { get; private set; }
    public string Barcode { get; private set; }
    public string Tags { get; private set; }
    public AverageRating AverageRating { get; private set; }
    public DateTime CreatedOn { get; private set; }
    public DateTime? ModifiedOn { get; private set; }

    public IReadOnlyList<ProductImage> ProductImages => _productImages.AsReadOnly();
    public IReadOnlyList<ProductReview> ProductReviews => _productReviews.AsReadOnly();

    public static Product Create(
        string name,
        string shortDescription,
        string description,
        CategoryId categoryId,
        string sku,
        Price unitPrice,
        decimal? discount,
        string brand,
        string barcode,
        string colors,
        string sizes,
        string tags
    )
    {
        return new Product(
            ProductId.CreateUnique(),
            name,
            shortDescription,
            description,
            categoryId,
            sku,
            0,
            unitPrice,
            discount,
            brand,
            barcode,
            colors,
            sizes,
            tags,
            AverageRating.CreateNew(0)
        );
    }

    public void Update(
        string name,
        string shortDescription,
        string description,
        CategoryId categoryId,
        string sku,
        Price unitPrice,
        decimal? discount,
        string brand,
        string barcode,
        string colors,
        string sizes,
        string tags
    )
    {
        Name = name;
        ShortDescription = shortDescription;
        Description = description;
        CategoryId = categoryId;
        SKU = sku;
        UnitPrice = unitPrice;
        Discount = discount;
        Brand = brand;
        Barcode = barcode;
        Colors = colors;
        Sizes = sizes;
        Tags = tags;
        ModifiedOn = DateTime.UtcNow;
    }

    public void AddProductImages(IList<ProductImage> productImages)
    {
        _productImages.AddRange(productImages);
    }

    public void AddProductImage(ProductImage productImage)
    {
        _productImages.Add(productImage);
    }

    public void RemoveProductImage(ProductImage productImage)
    {
        _productImages.Remove(productImage);
    }

    public void UpdateFavourite()
    {
        Favourites += 1;
    }

    private Product(
        ProductId productId,
        string name,
        string shortDescription,
        string description,
        CategoryId categoryId,
        string sku,
        int stockQuantity,
        Price unitPrice,
        decimal? discount,
        string brand,
        string barcode,
        string colors,
        string sizes,
        string tags,
        AverageRating averageRating
    )
        : base(productId)
    {
        Name = name;
        ShortDescription = shortDescription;
        Description = description;
        CategoryId = categoryId;
        SKU = sku;
        StockQuantity = stockQuantity;
        UnitPrice = unitPrice;
        Discount = discount;
        Brand = brand;
        Barcode = barcode;
        Colors = colors;
        Sizes = sizes;
        Tags = tags;
        AverageRating = averageRating;
        CreatedOn = DateTime.UtcNow;
    }

    private Product() { }
}
