using ErrorOr;
using MediatR;
using Shopizy.Application.Common.Caching;

namespace Shopizy.Application.Categories.Queries.CategoriesTree;

/// <summary>
/// Represents a query to retrieve categories in a hierarchical tree structure.
/// </summary>
public record CategoriesTreeQuery() : IRequest<ErrorOr<List<CategoryTree>>>, ICachableRequest
{
    public string CacheKey => "categories-tree";
    public TimeSpan? Expiration => TimeSpan.FromMinutes(30);
}
