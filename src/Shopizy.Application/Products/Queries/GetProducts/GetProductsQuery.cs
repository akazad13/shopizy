using ErrorOr;
using MediatR;
using Shopizy.Domain.Products;

namespace Shopizy.Application.Products.Queries.GetProducts;

/// <summary>
/// Represents a query to retrieve a paginated list of products with optional filtering.
/// </summary>
/// <param name="Name">Optional product name filter.</param>
/// <param name="CategoryIds">Optional list of category IDs to filter by.</param>
/// <param name="AverageRating">Optional minimum average rating filter.</param>
/// <param name="PageNumber">The page number.</param>
/// <param name="PageSize">The page size.</param>
public record GetProductsQuery(
    string? Name,
    IList<Guid>? CategoryIds,
    decimal? AverageRating,
    int PageNumber,
    int PageSize
) : IRequest<ErrorOr<List<Product>>>;
