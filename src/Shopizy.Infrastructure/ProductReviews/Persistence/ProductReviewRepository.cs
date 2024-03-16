using Microsoft.EntityFrameworkCore;
using Shopizy.Application.Common.Interfaces.Persistance;
using Shopizy.Domain.ProductReviews;
using Shopizy.Domain.ProductReviews.ValueObjects;
using Shopizy.Infrastructure.Common.Persistence;

namespace Shopizy.Infrastructure.ProductReviews.Persistence;

public class ProductReviewRepository(AppDbContext _dbContext) : IProductReviewRepository
{
    public Task<List<ProductReview>> GetProductReviewsAsync()
    {
        return _dbContext.ProductReviews.AsNoTracking().ToListAsync();
    }
    public Task<ProductReview?> GetProductReviewByIdAsync(ProductReviewId id)
    {
        return _dbContext.ProductReviews.FirstOrDefaultAsync(c => c.Id == id);
    }
    public async Task AddAsync(ProductReview productReview)
    {
        await _dbContext.ProductReviews.AddAsync(productReview);
    }
    public void Update(ProductReview productReview)
    {
        _dbContext.Update(productReview);
    }

    public Task<int> Commit(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}