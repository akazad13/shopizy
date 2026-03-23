using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.Products.Entities;
using Shopizy.Domain.Products.ValueObjects;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Products.Commands.UpdateVariant;

public class UpdateVariantCommandHandler(IProductRepository productRepository)
    : ICommandHandler<UpdateVariantCommand, ErrorOr<ProductVariant>>
{
    private readonly IProductRepository _productRepository = productRepository;

    public async Task<ErrorOr<ProductVariant>> Handle(
        UpdateVariantCommand cmd,
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

        var result = product.UpdateVariant(
            ProductVariantId.Create(cmd.VariantId),
            cmd.Name,
            cmd.SKU,
            Price.CreateNew(cmd.UnitPrice, cmd.Currency),
            cmd.StockQuantity,
            cmd.IsActive
        );

        if (result.IsError)
        {
            return result.Errors;
        }

        _productRepository.Update(product);

        return result.Value;
    }
}
