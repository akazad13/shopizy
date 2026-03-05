using System.Collections.Generic;
using System.Threading.Tasks;
using Shopizy.Domain.ProductReviews;
using Shopizy.Domain.ProductReviews.ValueObjects;

namespace Shopizy.Application.Common.Interfaces.Persistence;

public interface IProductReviewRepository
{
    Task<IReadOnlyList<ProductReview>> GetProductReviewsAsync();
    Task<ProductReview?> GetProductReviewByIdAsync(ProductReviewId id);
    Task AddAsync(ProductReview productReview);
    void Update(ProductReview productReview);
}
