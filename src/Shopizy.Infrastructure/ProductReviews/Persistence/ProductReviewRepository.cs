using Microsoft.EntityFrameworkCore;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.ProductReviews;
using Shopizy.Domain.ProductReviews.ValueObjects;
using Shopizy.Domain.Products.ValueObjects;
using Shopizy.Infrastructure.Common.Persistence;

namespace Shopizy.Infrastructure.ProductReviews.Persistence;

public class ProductReviewRepository(AppDbContext dbContext) : IProductReviewRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task<IReadOnlyList<ProductReview>> GetProductReviewsAsync() =>
        await _dbContext.ProductReviews.AsNoTracking().ToListAsync();

    public async Task<IReadOnlyList<ProductReview>> GetReviewsByProductIdAsync(
        ProductId productId,
        int pageNumber,
        int pageSize
    ) =>
        await _dbContext
            .ProductReviews.AsNoTracking()
            .Include(r => r.User)
            .Where(r => r.ProductId == productId)
            .OrderByDescending(r => r.CreatedOn)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

    public Task<ProductReview?> GetProductReviewByIdAsync(ProductReviewId id) =>
        _dbContext.ProductReviews.FirstOrDefaultAsync(c => c.Id == id);

    public async Task AddAsync(ProductReview productReview) =>
        await _dbContext.ProductReviews.AddAsync(productReview);

    public void Update(ProductReview productReview) =>
        _dbContext.ProductReviews.Update(productReview);

    public void Remove(ProductReview productReview) =>
        _dbContext.ProductReviews.Remove(productReview);
}
