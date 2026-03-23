using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.Products.Entities;
using Shopizy.Domain.Products.ValueObjects;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Products.Commands.AddVariant;

public class AddVariantCommandHandler(IProductRepository productRepository)
    : ICommandHandler<AddVariantCommand, ErrorOr<ProductVariant>>
{
    private readonly IProductRepository _productRepository = productRepository;

    public async Task<ErrorOr<ProductVariant>> Handle(
        AddVariantCommand cmd,
        CancellationToken cancellationToken
    )
    {
        var product = await _productRepository.GetProductByIdAsync(
            ProductId.Create(cmd.ProductId)
        );

        if (product is null)
        {
            return CustomErrors.Product.ProductNotFound;
        }

        var variant = ProductVariant.Create(
            cmd.Name,
            cmd.SKU,
            Price.CreateNew(cmd.UnitPrice, cmd.Currency),
            cmd.StockQuantity
        );

        product.AddVariant(variant);
        _productRepository.Update(product);

        return variant;
    }
}
