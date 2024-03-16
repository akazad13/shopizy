using ErrorOr;
using MediatR;
using Shopizy.Application.Common.Interfaces.Persistance;
using Shopizy.Domain.Products;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Application.Products.Queries.GetProduct;

public class GetProductQueryHandler(IProductRepository _productRepository) : IRequestHandler<GetProductQuery, ErrorOr<Product?>>
{
    public async Task<ErrorOr<Product?>> Handle(GetProductQuery request, CancellationToken cancellationToken)
    {
        return await _productRepository.GetProductByIdAsync(ProductId.Create(request.ProductId));
    }
}