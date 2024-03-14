using ErrorOr;
using MediatR;
using shopizy.Application.Common.Interfaces.Persistance;
using Shopizy.Domain.Products;

namespace shopizy.Application.Products.Queries.ListProducts;

public class ListProductQueryHandler(IProductRepository _productRepository) : IRequestHandler<ListProductQuery, ErrorOr<List<Product>?>>
{
    public async Task<ErrorOr<List<Product>?>> Handle(ListProductQuery request, CancellationToken cancellationToken)
    {
        return await _productRepository.GetProductsAsync();
    }
}