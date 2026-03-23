using Microsoft.EntityFrameworkCore;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.ProductQuestions;
using Shopizy.Domain.ProductQuestions.ValueObjects;
using Shopizy.Domain.Products.ValueObjects;
using Shopizy.Infrastructure.Common.Persistence;

namespace Shopizy.Infrastructure.ProductQuestions.Persistence;

public class ProductQuestionRepository(AppDbContext dbContext) : IProductQuestionRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task<IReadOnlyList<ProductQuestion>> GetByProductIdAsync(ProductId productId)
    {
        return await _dbContext.Set<ProductQuestion>()
            .Where(pq => pq.ProductId == productId)
            .ToListAsync();
    }

    public Task<ProductQuestion?> GetByIdAsync(ProductQuestionId id)
    {
        return _dbContext.Set<ProductQuestion>()
            .FirstOrDefaultAsync(pq => pq.Id == id);
    }

    public async Task AddAsync(ProductQuestion question)
    {
        await _dbContext.Set<ProductQuestion>().AddAsync(question);
    }

    public void Update(ProductQuestion question)
    {
        _dbContext.Set<ProductQuestion>().Update(question);
    }

    public void Remove(ProductQuestion question)
    {
        _dbContext.Set<ProductQuestion>().Remove(question);
    }
}
