using System.Collections.Generic;
using ErrorOr;
using MediatR;
using Shopizy.SharedKernel.Application.Caching;
using Shopizy.Domain.Categories;

namespace Shopizy.Application.Categories.Queries.ListCategories;

/// <summary>
/// Represents a query to retrieve all categories.
/// </summary>
public record ListCategoriesQuery() : IRequest<ErrorOr<List<Category>>>, ICachableRequest
{
    public string CacheKey => "categories-all";
    public TimeSpan? Expiration => TimeSpan.FromMinutes(30);
}
