using Ardalis.GuardClauses;
using MediatR;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Wrappers;
using Shopizy.Domain.Categories.ValueObjects;
using Shopizy.Domain.Products;

namespace Shopizy.Application.Products.Queries.GetProducts;

public class GetProductsQueryHandler(IProductRepository productRepository)
    : IRequestHandler<GetProductsQuery, IResult<List<Product>?>>
{
    private readonly IProductRepository _productRepository = productRepository;

    public async Task<IResult<List<Product>?>> Handle(
        GetProductsQuery query,
        CancellationToken cancellationToken
    )
    {
        Guard.Against.Null(query);

        var products = await _productRepository.GetProductsAsync(
            query.Name,
            query
                .CategoryIds?.AsQueryable()
                .Select(categoryId => CategoryId.Create(categoryId))
                .ToList(),
            query.AverageRating,
            query.PageNumber,
            query.PageSize
        );
        return Response<List<Product>?>.SuccessResponese(products);
    }
}
