using Shopizy.Domain.ProductReviews;
using Shopizy.Domain.ProductReviews.ValueObjects;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Application.Common.Interfaces.Persistence;

public interface IProductReviewRepository
{
    Task<IReadOnlyList<ProductReview>> GetProductReviewsAsync();
    Task<IReadOnlyList<ProductReview>> GetReviewsByProductIdAsync(ProductId productId);
    Task<ProductReview?> GetProductReviewByIdAsync(ProductReviewId id);
    Task AddAsync(ProductReview productReview);
    void Update(ProductReview productReview);
    void Remove(ProductReview productReview);
}

