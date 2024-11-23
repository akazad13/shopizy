using ErrorOr;
using MediatR;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Products;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Application.Products.Queries.GetProduct;

public class GetProductQueryHandler(IProductRepository productRepository)
    : IRequestHandler<GetProductQuery, ErrorOr<Product>>
{
    private readonly IProductRepository _productRepository = productRepository;

    public async Task<ErrorOr<Product>> Handle(
        GetProductQuery query,
        CancellationToken cancellationToken
    )
    {
        var product = await _productRepository.GetProductByIdAsync(
            ProductId.Create(query.ProductId)
        );
        if (product is null)
        {
            return CustomErrors.Product.ProductNotFound;
        }
        return product;
    }
}
