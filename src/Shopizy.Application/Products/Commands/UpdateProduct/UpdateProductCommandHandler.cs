using ErrorOr;
using MediatR;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Categories.ValueObjects;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Application.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler(IProductRepository productRepository)
    : IRequestHandler<UpdateProductCommand, ErrorOr<Success>>
{
    private readonly IProductRepository _productRepository = productRepository;

    public async Task<ErrorOr<Success>> Handle(
        UpdateProductCommand cmd,
        CancellationToken cancellationToken
    )
    {
        var product = await _productRepository.GetProductByIdAsync(ProductId.Create(cmd.ProductId));

        if (product is null)
        {
            return CustomErrors.Product.ProductNotFound;
        }

        product.Update(
            name: cmd.Name,
            shortDescription: cmd.ShortDescription,
            description: cmd.Description,
            categoryId: CategoryId.Create(cmd.CategoryId),
            sku: cmd.Sku,
            unitPrice: Price.CreateNew(cmd.UnitPrice, cmd.Currency),
            discount: cmd.Discount,
            brand: cmd.Brand,
            barcode: cmd.Barcode,
            colors: cmd.Colors,
            sizes: cmd.Sizes,
            tags: cmd.Tags
        );

        _productRepository.Update(product);

        if (await _productRepository.Commit(cancellationToken) <= 0)
        {
            return CustomErrors.Product.ProductNotUpdated;
        }

        return Result.Success;
    }
}
