using Ardalis.GuardClauses;
using ErrorOr;
using MediatR;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Categories.ValueObjects;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Products;

namespace Shopizy.Application.Products.Queries.GetProducts;

public class GetProductsQueryHandler(IProductRepository productRepository)
    : IRequestHandler<GetProductsQuery, ErrorOr<List<Product>>>
{
    private readonly IProductRepository _productRepository = productRepository;

    public async Task<ErrorOr<List<Product>>> Handle(
        GetProductsQuery query,
        CancellationToken cancellationToken
    )
    {
        Guard.Against.Null(query);

        if (query.PageNumber <= 0) query = query with { PageNumber = 1 };
        if (query.PageSize <= 0) query = query with { PageSize = 10 };

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
        if (products == null)
        {
            return CustomErrors.Product.ProductNotFound;
        }
        return products;
    }
}
