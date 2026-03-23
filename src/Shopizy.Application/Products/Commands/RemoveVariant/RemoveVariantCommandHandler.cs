using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Products.ValueObjects;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Products.Commands.RemoveVariant;

public class RemoveVariantCommandHandler(IProductRepository productRepository)
    : ICommandHandler<RemoveVariantCommand, ErrorOr<Deleted>>
{
    private readonly IProductRepository _productRepository = productRepository;

    public async Task<ErrorOr<Deleted>> Handle(
        RemoveVariantCommand cmd,
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

        var result = product.RemoveVariant(ProductVariantId.Create(cmd.VariantId));

        if (result.IsError)
        {
            return result.Errors;
        }

        _productRepository.Update(product);

        return Result.Deleted;
    }
}
