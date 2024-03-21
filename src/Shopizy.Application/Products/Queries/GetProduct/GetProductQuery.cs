using ErrorOr;
using MediatR;
using Shopizy.Domain.Products;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Application.Products.Queries.GetProduct;

public record GetProductQuery(ProductId ProductId) : IRequest<ErrorOr<Product?>>;
