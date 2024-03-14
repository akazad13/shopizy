using ErrorOr;
using MediatR;
using shopizy.Application.Common.Interfaces.Persistance;
using Shopizy.Domain.Categories.ValueObjects;
using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.Products;
using Shopizy.Domain.Common.Errors;

namespace shopizy.Application.Products.Commands.CreateProduct;

public class CreateProductCommandHandler(IProductRepository _productRepository) : IRequestHandler<CreateProductCommand, ErrorOr<Product>>
{
    public async Task<ErrorOr<Product>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = Product.Create(request.Name, request.Description, CategoryId.Create(request.CategoryId), request.Sku, request.StockQuantity, Price.CreateNew(request.UnitPrice, request.Currency), request.Discount, request.Brand, request.Barcode, request.Tags, "");
        await _productRepository.AddAsync(product);

        if(await _productRepository.Commit(cancellationToken) <= 0)
            return Errors.Product.ProductNotCreated;

        return product;

    }
}