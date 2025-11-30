namespace Shopizy.Contracts.Product;

/// <summary>
/// Represents a summary of a product.
/// </summary>
/// <param name="ProductId">The unique identifier of the product.</param>
/// <param name="Name">The name of the product.</param>
/// <param name="ShortDescription">A short description.</param>
/// <param name="Description">The full description.</param>
/// <param name="CategoryId">The category identifier.</param>
/// <param name="Price">The formatted price.</param>
/// <param name="Discount">The discount amount.</param>
/// <param name="Brand">The brand name.</param>
/// <param name="Sizes">Available sizes.</param>
/// <param name="Colors">Available colors.</param>
/// <param name="Tags">Product tags.</param>
/// <param name="Barcode">The barcode.</param>
/// <param name="StockQuantity">The available stock quantity.</param>
/// <param name="AverageRating">The average rating.</param>
/// <param name="ProductImages">The list of product images.</param>
public record ProductResponse(
    Guid ProductId,
    string Name,
    string ShortDescription,
    string Description,
    Guid CategoryId,
    string Price,
    decimal Discount,
    string Brand,
    string Sizes,
    string Colors,
    string Tags,
    string Barcode,
    int StockQuantity,
    AverageRating AverageRating,
    IList<ProductImageResponse> ProductImages
);

/// <summary>
/// Represents detailed information about a product.
/// </summary>
/// <param name="ProductId">The unique identifier of the product.</param>
/// <param name="Name">The name of the product.</param>
/// <param name="ShortDescription">A short description.</param>
/// <param name="Description">The full description.</param>
/// <param name="CategoryId">The category identifier.</param>
/// <param name="Price">The formatted price.</param>
/// <param name="Discount">The discount amount.</param>
/// <param name="Sku">The stock keeping unit.</param>
/// <param name="Brand">The brand name.</param>
/// <param name="Sizes">Available sizes.</param>
/// <param name="Colors">Available colors.</param>
/// <param name="Tags">Product tags.</param>
/// <param name="Barcode">The barcode.</param>
/// <param name="StockQuantity">The available stock quantity.</param>
/// <param name="AverageRating">The average rating.</param>
/// <param name="Favourites">The number of times favorited.</param>
/// <param name="Specifications">The list of specifications.</param>
/// <param name="ProductImages">The list of product images.</param>
/// <param name="ProductReviews">The list of product reviews.</param>
public record ProductDetailResponse(
    Guid ProductId,
    string Name,
    string ShortDescription,
    string Description,
    Guid CategoryId,
    string Price,
    decimal Discount,
    string Sku,
    string Brand,
    string Sizes,
    string Colors,
    string Tags,
    string Barcode,
    int StockQuantity,
    AverageRating AverageRating,
    int Favourites,
    IList<string>? Specifications,
    IList<ProductImageResponse> ProductImages,
    IList<ProductReviewResponse> ProductReviews
);

/// <summary>
/// Represents a product image.
/// </summary>
/// <param name="ProductImageId">The unique identifier of the image.</param>
/// <param name="ImageUrl">The URL of the image.</param>
/// <param name="Seq">The display sequence.</param>
/// <param name="PublicId">The public identifier (e.g., in cloud storage).</param>
public record ProductImageResponse(Guid ProductImageId, string ImageUrl, int Seq, string PublicId);

/// <summary>
/// Represents a product review.
/// </summary>
/// <param name="ProductReviewId">The unique identifier of the review.</param>
/// <param name="Reviewer">The name of the reviewer.</param>
/// <param name="ReviewerImageUrl">The URL of the reviewer's image.</param>
/// <param name="Comment">The review comment.</param>
/// <param name="Rating">The rating given.</param>
/// <param name="CreatedOn">The date the review was created.</param>
public record ProductReviewResponse(
    Guid ProductReviewId,
    string Reviewer,
    string? ReviewerImageUrl,
    string Comment,
    decimal Rating,
    DateTime CreatedOn
);

/// <summary>
/// Represents the average rating of a product.
/// </summary>
/// <param name="Value">The average rating value.</param>
/// <param name="NumRatings">The total number of ratings.</param>
public record AverageRating(decimal Value, int NumRatings);
