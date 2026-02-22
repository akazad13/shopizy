using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Categories;
using Shopizy.Domain.Categories.ValueObjects;
using Shopizy.Infrastructure.Categories.Specifications;
using Shopizy.Infrastructure.Common.Persistence;
using Shopizy.Infrastructure.Common.Specifications;

namespace Shopizy.Infrastructure.Categories.Persistence;

/// <summary>
/// Repository for managing category data persistence.
/// </summary>
public class CategoryRepository(AppDbContext dbContext) : ICategoryRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    /// <summary>
    /// Checks if a category exists by name.
    /// </summary>
    /// <param name="name">The category name.</param>
    /// <returns>True if a category with the name exists; otherwise, false.</returns>
    public Task<bool> GetCategoryByNameAsync(string name)
    {
        return _dbContext.Categories.AnyAsync(category => category.Name == name);
    }

    /// <summary>
    /// Retrieves a category by its unique identifier.
    /// </summary>
    /// <param name="id">The category identifier.</param>
    /// <returns>The category if found; otherwise, null.</returns>
    public Task<Category?> GetCategoryByIdAsync(CategoryId id)
    {
        return ApplySpec(new CategoryByIdSpec(id)).FirstOrDefaultAsync();
    }

    /// <summary>
    /// Retrieves all categories from the database.
    /// </summary>
    /// <returns>A list of all categories.</returns>
    public async Task<IReadOnlyList<Category>> GetCategoriesAsync()
    {
        var categoryList = await _dbContext.Categories.AsNoTracking().ToListAsync();
        return categoryList;
    }

    /// <summary>
    /// Adds a new category to the database.
    /// </summary>
    /// <param name="category">The category to add.</param>
    public async Task AddAsync(Category category)
    {
        await _dbContext.Categories.AddAsync(category);
    }

    /// <summary>
    /// Updates an existing category in the database.
    /// </summary>
    /// <param name="category">The category to update.</param>
    public void Update(Category category)
    {
        _dbContext.Update(category);
    }

    /// <summary>
    /// Removes a category from the database.
    /// </summary>
    /// <param name="category">The category to remove.</param>
    public void Remove(Category category)
    {
        _dbContext.Remove(category);
    }

    /// <summary>
    /// Applies a specification to the category query.
    /// </summary>
    /// <param name="spec">The specification to apply.</param>
    /// <returns>A queryable of categories.</returns>
    private IQueryable<Category> ApplySpec(Specification<Category> spec)
    {
        return SpecificationEvaluator.GetQuery(_dbContext.Categories, spec);
    }
}
