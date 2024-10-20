using ErrorOr;
using MediatR;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Categories.ValueObjects;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.Products;

namespace Shopizy.Application.Products.Commands.CreateProduct;

public class CreateProductCommandHandler(IProductRepository productRepository)
    : IRequestHandler<CreateProductCommand, ErrorOr<Product>>
{
    private readonly IProductRepository _productRepository = productRepository;

    public async Task<ErrorOr<Product>> Handle(
        CreateProductCommand cmd,
        CancellationToken cancellationToken
    )
    {
        var product = Product.Create(
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

        await _productRepository.AddAsync(product);

        if (await _productRepository.Commit(cancellationToken) <= 0)
        {
            return CustomErrors.Product.ProductNotCreated;
        }

        return product;
    }
}
