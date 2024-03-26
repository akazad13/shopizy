using Microsoft.EntityFrameworkCore;
using Shopizy.Application.Common.Interfaces.Persistance;
using Shopizy.Infrastructure.Categories.Specifications;
using Shopizy.Infrastructure.Common.Specifications;
using Shopizy.Domain.Categories;
using Shopizy.Domain.Categories.ValueObjects;
using Shopizy.Infrastructure.Common.Persistence;

namespace Shopizy.Infrastructure.Categories.Persistence;

public class CategoryRepository(AppDbContext _dbContext) : ICategoryRepository
{
    public Task<bool> GetCategoryByNameAsync(string name)
    {
        return _dbContext.Categories.AnyAsync(category => category.Name == name);
    }

    public Task<Category?> GetCategoryByIdAsync(CategoryId id)
    {
        return ApplySpec(new CategoryByIdSpec(id)).FirstOrDefaultAsync();
    }
    public Task<List<Category>> GetCategoriesAsync()
    {
        return _dbContext.Categories.AsNoTracking().ToListAsync();
    }

    public async Task AddAsync(Category category)
    {
        await _dbContext.Categories.AddAsync(category);
    }

    public void Update(Category category)
    {
        _dbContext.Update(category);
    }

    public void Remove(Category category)
    {
        _dbContext.Remove(category);
    }

    public Task<int> Commit(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
    private IQueryable<Category> ApplySpec(Specification<Category> spec)
    {
        return SpecificationEvaluator.GetQuery(_dbContext.Categories, spec);
    }
}