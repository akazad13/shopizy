using Shopizy.Domain.Categories;
using Shopizy.Domain.Categories.ValueObjects;

namespace shopizy.Application.Common.Interfaces.Persistance;

public interface ICategoryRepository
{
    Task<bool> GetCategoryByNameAsync(string name);
    Task<Category?> GetCategoryByIdAsync(CategoryId id);
    Task<List<Category>> GetCategoriesAsync();
    Task AddAsync(Category category);
    Task<int> Commit(CancellationToken cancellationToken);
}
