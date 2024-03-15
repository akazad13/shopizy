using Shopizy.Domain.ProductReviews;
using Shopizy.Domain.ProductReviews.ValueObjects;

namespace shopizy.Application.Common.Interfaces.Persistance;

public interface IProductReviewRepository
{
    Task<List<ProductReview>> GetProductReviewsAsync();
    Task<ProductReview?> GetProductReviewByIdAsync(ProductReviewId id);
    Task AddAsync(ProductReview productReview);
    void Update(ProductReview productReview);
    Task<int> Commit(CancellationToken cancellationToken);
}
