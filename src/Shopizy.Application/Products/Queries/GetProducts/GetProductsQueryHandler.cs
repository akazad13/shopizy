using ErrorOr;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Categories.ValueObjects;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Application.Products.Queries.GetProducts;

public class GetProductsQueryHandler(IProductRepository productRepository)
    : IQueryHandler<GetProductsQuery, ErrorOr<ProductsResult>>
{
    private readonly IProductRepository _productRepository = productRepository;

    public async Task<ErrorOr<ProductsResult>> Handle(
        GetProductsQuery query,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(query);

        if (query.PageNumber <= 0)
        {
            query = query with { PageNumber = 1 };
        }

        if (query.PageSize <= 0)
        {
            query = query with { PageSize = 10 };
        }

        var categoryIds = query.CategoryIds?.Any() == true
            ? query.CategoryIds
                .Select(categoryId => CategoryId.Create(categoryId))
                .ToList()
            : null;

        var productIds = query.ProductIds?.Any() == true
            ? query.ProductIds
                .Select(productId => ProductId.Create(productId))
                .ToList()
            : null;

        var productsTask = _productRepository.GetProductsAsync(
            productIds,
            query.Name,
            categoryIds,
            query.AverageRating,
            query.PageNumber,
            query.PageSize
        );
        var countTask = _productRepository.CountProductsAsync(
            productIds,
            query.Name,
            categoryIds,
            query.AverageRating
        );

        await Task.WhenAll(productsTask, countTask);

        var products = productsTask.Result;
        var totalCount = countTask.Result;

        if (products == null)
        {
            return CustomErrors.Product.ProductNotFound;
        }

        return new ProductsResult(products.ToList(), totalCount);
    }
}
