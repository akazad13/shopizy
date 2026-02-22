using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.ProductReviews;
using Shopizy.Domain.ProductReviews.ValueObjects;
using Shopizy.Infrastructure.Common.Persistence;

namespace Shopizy.Infrastructure.ProductReviews.Persistence;

public class ProductReviewRepository(AppDbContext dbContext) : IProductReviewRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task<IReadOnlyList<ProductReview>> GetProductReviewsAsync()
    {
        return await _dbContext.ProductReviews.AsNoTracking().ToListAsync();
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
        _dbContext.ProductReviews.Update(productReview);
    }
}
