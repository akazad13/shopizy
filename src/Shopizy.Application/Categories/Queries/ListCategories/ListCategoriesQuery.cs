using ErrorOr;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.SharedKernel.Application.Caching;
using Shopizy.Domain.Categories;

namespace Shopizy.Application.Categories.Queries.ListCategories;

/// <summary>
/// Represents a query to retrieve all categories.
/// </summary>
public record ListCategoriesQuery() : IQuery<ErrorOr<List<Category>>>, ICachableRequest
{
    public string CacheKey => "categories-all";
    public TimeSpan? Expiration => TimeSpan.FromMinutes(30);
}
