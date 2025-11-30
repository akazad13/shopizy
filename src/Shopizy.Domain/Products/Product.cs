using Shopizy.Domain.Categories.ValueObjects;
using Shopizy.Domain.Common.Models;
using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.ProductReviews;
using Shopizy.Domain.Products.Entities;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Domain.Products;

/// <summary>
/// Represents a product in the catalog.
/// </summary>
public sealed class Product : AggregateRoot<ProductId, Guid>
{
    private readonly List<ProductImage> _productImages = [];
    private readonly List<ProductReview> _productReviews = [];
    
    /// <summary>
    /// Gets the product name.
    /// </summary>
    public string Name { get; private set; }
    
    /// <summary>
    /// Gets the short description of the product.
    /// </summary>
    public string ShortDescription { get; private set; }
    
    /// <summary>
    /// Gets the detailed description of the product.
    /// </summary>
    public string Description { get; private set; }
    
    /// <summary>
    /// Gets the category ID this product belongs to.
    /// </summary>
    public CategoryId CategoryId { get; private set; }
    
    /// <summary>
    /// Gets the stock keeping unit (SKU).
    /// </summary>
    public string SKU { get; private set; }
    
    /// <summary>
    /// Gets the current stock quantity.
    /// </summary>
    public int StockQuantity { get; private set; }
    
    /// <summary>
    /// Gets the unit price of the product.
    /// </summary>
    public Price UnitPrice { get; private set; }
    
    /// <summary>
    /// Gets the discount percentage.
    /// </summary>
    public decimal? Discount { get; private set; }
    
    /// <summary>
    /// Gets the product brand.
    /// </summary>
    public string Brand { get; private set; }
    
    /// <summary>
    /// Gets the available colors (comma-separated).
    /// </summary>
    public string Colors { get; private set; }
    
    /// <summary>
    /// Gets the available sizes (comma-separated).
    /// </summary>
    public string Sizes { get; private set; }
    
    /// <summary>
    /// Gets the number of times this product has been favorited.
    /// </summary>
    public int Favourites { get; private set; }
    
    /// <summary>
    /// Gets the product barcode.
    /// </summary>
    public string Barcode { get; private set; }
    
    /// <summary>
    /// Gets the product tags (comma-separated).
    /// </summary>
    public string Tags { get; private set; }
    
    /// <summary>
    /// Gets the average rating of the product.
    /// </summary>
    public AverageRating AverageRating { get; private set; }
    
    /// <summary>
    /// Gets the date and time when the product was created.
    /// </summary>
    public DateTime CreatedOn { get; private set; }
    
    /// <summary>
    /// Gets the date and time when the product was last modified.
    /// </summary>
    public DateTime? ModifiedOn { get; private set; }

    /// <summary>
    /// Gets the read-only list of product images.
    /// </summary>
    public IReadOnlyList<ProductImage> ProductImages => _productImages.AsReadOnly();
    
    /// <summary>
    /// Gets the read-only list of product reviews.
    /// </summary>
    public IReadOnlyList<ProductReview> ProductReviews => _productReviews.AsReadOnly();

    /// <summary>
    /// Creates a new product.
    /// </summary>
    /// <param name="name">The product name.</param>
    /// <param name="shortDescription">A brief description.</param>
    /// <param name="description">The full product description.</param>
    /// <param name="categoryId">The category identifier.</param>
    /// <param name="sku">The stock keeping unit.</param>
    /// <param name="unitPrice">The unit price.</param>
    /// <param name="discount">The discount percentage.</param>
    /// <param name="brand">The brand name.</param>
    /// <param name="barcode">The product barcode.</param>
    /// <param name="colors">Available colors.</param>
    /// <param name="sizes">Available sizes.</param>
    /// <param name="tags">Product tags.</param>
    /// <returns>A new <see cref="Product"/> instance.</returns>
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

    /// <summary>
    /// Updates the product information.
    /// </summary>
    /// <param name="name">The product name.</param>
    /// <param name="shortDescription">A brief description.</param>
    /// <param name="description">The full product description.</param>
    /// <param name="categoryId">The category identifier.</param>
    /// <param name="sku">The stock keeping unit.</param>
    /// <param name="unitPrice">The unit price.</param>
    /// <param name="discount">The discount percentage.</param>
    /// <param name="brand">The brand name.</param>
    /// <param name="barcode">The product barcode.</param>
    /// <param name="colors">Available colors.</param>
    /// <param name="sizes">Available sizes.</param>
    /// <param name="tags">Product tags.</param>
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

    /// <summary>
    /// Adds multiple product images.
    /// </summary>
    /// <param name="productImages">The list of product images to add.</param>
    public void AddProductImages(IList<ProductImage> productImages)
    {
        _productImages.AddRange(productImages);
    }

    /// <summary>
    /// Adds a single product image.
    /// </summary>
    /// <param name="productImage">The product image to add.</param>
    public void AddProductImage(ProductImage productImage)
    {
        _productImages.Add(productImage);
    }

    /// <summary>
    /// Removes a product image.
    /// </summary>
    /// <param name="productImage">The product image to remove.</param>
    public void RemoveProductImage(ProductImage productImage)
    {
        _productImages.Remove(productImage);
    }

    /// <summary>
    /// Increments the favorite count for this product.
    /// </summary>
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
