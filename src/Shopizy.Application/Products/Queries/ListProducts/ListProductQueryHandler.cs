using ErrorOr;
using MediatR;
using Shopizy.Application.Common.Interfaces.Persistance;
using Shopizy.Domain.Products;

namespace Shopizy.Application.Products.Queries.ListProducts;

public class ListProductQueryHandler(IProductRepository productRepository) : IRequestHandler<ListProductQuery, ErrorOr<List<Product>?>>
{
    private readonly IProductRepository _productRepository = productRepository;
    public async Task<ErrorOr<List<Product>?>> Handle(ListProductQuery query, CancellationToken cancellationToken)
    {
        return await _productRepository.GetProductsAsync();
    }
}