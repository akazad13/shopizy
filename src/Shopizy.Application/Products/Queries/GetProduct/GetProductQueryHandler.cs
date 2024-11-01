using MediatR;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Wrappers;
using Shopizy.Domain.Products;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Application.Products.Queries.GetProduct;

public class GetProductQueryHandler(IProductRepository productRepository)
    : IRequestHandler<GetProductQuery, IResult<Product?>>
{
    private readonly IProductRepository _productRepository = productRepository;

    public async Task<IResult<Product?>> Handle(
        GetProductQuery query,
        CancellationToken cancellationToken
    )
    {
        var product = await _productRepository.GetProductByIdAsync(
            ProductId.Create(query.ProductId)
        );
        return Response<Product?>.SuccessResponese(product);
    }
}
