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
            cmd.Name,
            cmd.Description,
            CategoryId.Create(cmd.CategoryId),
            cmd.Sku,
            Price.CreateNew(cmd.UnitPrice, cmd.Currency),
            cmd.Discount,
            cmd.Brand,
            cmd.Barcode,
            cmd.Tags
        );

        _productRepository.Update(product);

        if (await _productRepository.Commit(cancellationToken) <= 0)
        {
            return CustomErrors.Product.ProductNotUpdated;
        }

        return Result.Success;
    }
}
