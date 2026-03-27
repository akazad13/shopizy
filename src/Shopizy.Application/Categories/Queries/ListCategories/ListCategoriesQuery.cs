using ErrorOr;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.SharedKernel.Application.Caching;

namespace Shopizy.Application.Categories.Queries.ListCategories;

/// <summary>
/// Represents a query to retrieve all categories.
/// </summary>
public record ListCategoriesQuery() : IQuery<ErrorOr<List<CategoryItem>>>, ICachableRequest
{
    public string CacheKey => "categories-all";
    public TimeSpan? Expiration => TimeSpan.FromMinutes(30);
}
