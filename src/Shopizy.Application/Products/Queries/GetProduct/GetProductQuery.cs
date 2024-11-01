using MediatR;
using Shopizy.Application.Common.Wrappers;
using Shopizy.Domain.Products;

namespace Shopizy.Application.Products.Queries.GetProduct;

public record GetProductQuery(Guid ProductId) : IRequest<IResult<Product?>>;
