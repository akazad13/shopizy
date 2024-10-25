using MediatR;
using Shopizy.Application.Common.Wrappers;
using Shopizy.Domain.Products;

namespace Shopizy.Application.Products.Queries.ListProducts;

public record ListProductsQuery() : IRequest<IResult<List<Product>?>>;
