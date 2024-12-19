using ErrorOr;
using MediatR;
using Shopizy.Domain.Products;

namespace Shopizy.Application.Products.Queries.GetProducts;

public record GetProductsQuery(
    string? Name,
    IList<Guid>? CategoryIds,
    decimal? AverageRating,
    int PageNumber,
    int PageSize
) : IRequest<ErrorOr<List<Product>>>;
