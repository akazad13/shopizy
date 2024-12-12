namespace Shopizy.Contracts.Product;

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

public record ProductImageResponse(Guid ProductImageId, string ImageUrl, int Seq, string PublicId);

public record ProductReviewResponse(
    Guid ProductReviewId,
    string Reviewer,
    string? ReviewerImageUrl,
    string Comment,
    decimal Rating,
    DateTime CreatedOn
);

public record AverageRating(decimal Value, int NumRatings);
