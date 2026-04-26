using Shopizy.Domain.Brands;
using Shopizy.Domain.Brands.ValueObjects;

namespace Shopizy.Application.Common.Interfaces.Persistence;

public interface IBrandRepository
{
    Task<Brand?> GetByIdAsync(BrandId id);
    Task<Brand?> GetByNameAsync(string name);
    Task<IReadOnlyList<Brand>> GetAsync();
    Task AddAsync(Brand brand);
    void Remove(Brand brand);
}