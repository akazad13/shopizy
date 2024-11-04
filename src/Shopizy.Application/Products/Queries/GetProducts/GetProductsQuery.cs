using MediatR;
using Shopizy.Application.Common.Wrappers;
using Shopizy.Domain.Products;

namespace Shopizy.Application.Products.Queries.GetProducts;

public record GetProductsQuery(
    string? Name,
    IList<Guid>? CategoryIds,
    double? AverageRating,
    int PageNumber,
    int PageSize
) : IRequest<IResult<List<Product>?>>;
