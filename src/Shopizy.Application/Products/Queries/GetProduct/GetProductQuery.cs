using ErrorOr;
using MediatR;
using Shopizy.Domain.Products;

namespace shopizy.Application.Products.Queries.GetProduct;

public record GetProductQuery(Guid ProductId) : IRequest<ErrorOr<Product?>>;
