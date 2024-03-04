using Microsoft.EntityFrameworkCore;
using shopizy.Application.Common.Interfaces.Persistance;
using Shopizy.Domain.Categories;
using Shopizy.Infrastructure.Common.Persistence;

namespace Shopizy.Infrastructure.Categories.Persistence;

public class CategoryRepository(AppDbContext _dbContext) : ICategoryRepository
{
    public Task<bool> GetCategoryByNameAsync(string name)
    {
        return _dbContext.Categories.AnyAsync(category => category.Name == name);
    }
    public async Task AddAsync(Category category)
    {
        await _dbContext.Categories.AddAsync(category);
    }

    public Task<int> Commit(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
