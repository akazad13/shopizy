using ErrorOr;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.SharedKernel.Application.Caching;

namespace Shopizy.Application.Categories.Queries.CategoriesTree;

/// <summary>
/// Represents a query to retrieve categories in a hierarchical tree structure.
/// </summary>
public record CategoriesTreeQuery() : IQuery<ErrorOr<List<CategoryTree>>>, ICachableRequest
{
    public string CacheKey => "categories-tree";
    public TimeSpan? Expiration => TimeSpan.FromMinutes(30);
}
