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

        await _productRepository.AddAsync(product);

        if (await _productRepository.Commit(cancellationToken) <= 0)
        {
            return CustomErrors.Product.ProductNotCreated;
        }

        return product;
    }
}
