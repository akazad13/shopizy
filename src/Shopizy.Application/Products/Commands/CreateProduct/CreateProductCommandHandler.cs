using ErrorOr;
using MediatR;
using Shopizy.Application.Common.Interfaces.Persistance;
using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.Products;
using Shopizy.Domain.Common.Errors;
using Shopizy.Domain.Categories.ValueObjects;

namespace Shopizy.Application.Products.Commands.CreateProduct;

public class CreateProductCommandHandler(IProductRepository _productRepository)
        : IRequestHandler<CreateProductCommand, ErrorOr<Product>>
{
    public async Task<ErrorOr<Product>> Handle(CreateProductCommand cmd, CancellationToken cancellationToken)
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
            cmd.Tags, ""
        );

        await _productRepository.AddAsync(product);

        if (await _productRepository.Commit(cancellationToken) <= 0)
            return Errors.Product.ProductNotCreated;

        return product;

    }
}