using ErrorOr;
using MediatR;
using Shopizy.Domain.Products;

namespace Shopizy.Application.Products.Queries.GetProduct;

public record GetProductQuery(Guid ProductId) : IRequest<ErrorOr<Product>>;
