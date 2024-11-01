using MediatR;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Wrappers;
using Shopizy.Domain.Categories.ValueObjects;
using Shopizy.Domain.Products;

namespace Shopizy.Application.Products.Queries.ListProducts;

public class ListProductsQueryHandler(IProductRepository productRepository)
    : IRequestHandler<ListProductsQuery, IResult<List<Product>?>>
{
    private readonly IProductRepository _productRepository = productRepository;

    public async Task<IResult<List<Product>?>> Handle(
        ListProductsQuery query,
        CancellationToken cancellationToken
    )
    {
        var products = await _productRepository.GetProductsAsync(
            query.Name,
            query
                .CategoryIds?.AsQueryable()
                .Select(categoryId => CategoryId.Create(categoryId))
                .ToList(),
            query.AverageRating
        );
        return Response<List<Product>?>.SuccessResponese(products);
    }
}
