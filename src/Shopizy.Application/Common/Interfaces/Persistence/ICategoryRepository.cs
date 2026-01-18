using Shopizy.Domain.Categories;
using Shopizy.Domain.Categories.ValueObjects;

namespace Shopizy.Application.Common.Interfaces.Persistence;

public interface ICategoryRepository
{
    Task<bool> GetCategoryByNameAsync(string name);
    Task<Category?> GetCategoryByIdAsync(CategoryId id);
    Task<List<Category>> GetCategoriesAsync();
    Task AddAsync(Category category);
    void Update(Category category);

    void Remove(Category category);

    Task<int> CommitAsync(CancellationToken cancellationToken);
}
