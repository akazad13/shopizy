using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Products.Entities;
using Shopizy.Domain.Products.ValueObjects;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Products.Queries.GetProductVariants;

public class GetProductVariantsQueryHandler(IProductRepository productRepository)
    : IQueryHandler<GetProductVariantsQuery, ErrorOr<IReadOnlyList<ProductVariant>>>
{
    private readonly IProductRepository _productRepository = productRepository;

    public async Task<ErrorOr<IReadOnlyList<ProductVariant>>> Handle(
        GetProductVariantsQuery query,
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

        return ErrorOrFactory.From(product.ProductVariants);
    }
}
