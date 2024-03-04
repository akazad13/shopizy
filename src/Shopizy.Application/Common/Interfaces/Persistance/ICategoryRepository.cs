using Shopizy.Domain.Categories;

namespace shopizy.Application.Common.Interfaces.Persistance;

public interface ICategoryRepository
{
    Task<bool> GetCategoryByNameAsync(string name);
    Task AddAsync(Category category);
    Task<int> Commit(CancellationToken cancellationToken);
}
