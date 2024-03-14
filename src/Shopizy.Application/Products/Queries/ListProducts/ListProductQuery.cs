using ErrorOr;
using MediatR;
using Shopizy.Domain.Products;

namespace shopizy.Application.Products.Queries.ListProducts;

public record ListProductQuery() : IRequest<ErrorOr<List<Product>?>>;
