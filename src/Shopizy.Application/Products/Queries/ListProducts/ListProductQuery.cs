using ErrorOr;
using MediatR;
using Shopizy.Domain.Products;

namespace Shopizy.Application.Products.Queries.ListProducts;

public record ListProductQuery() : IRequest<ErrorOr<List<Product>?>>;
