using ErrorOr;
using MediatR;
using Shopizy.Application.Common.Interfaces.Persistance;
using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.Products;
using Shopizy.Domain.Common.Errors;
using Shopizy.Domain.Categories.ValueObjects;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Domain.Products.Entities;

namespace Shopizy.Application.Products.Commands.CreateProduct;

public class CreateProductCommandHandler(IProductRepository _productRepository, IMediaUploader _mediaUploader)
        : IRequestHandler<CreateProductCommand, ErrorOr<Product>>
{
    public async Task<ErrorOr<Product>> Handle(CreateProductCommand cmd, CancellationToken cancellationToken)
    {
        var productImages = new List<ProductImage>();
        if (cmd.Images != null)
        {
            foreach (var image in cmd.Images)
            {
                var res = await _mediaUploader.UploadPhotoAsync(image, cancellationToken);
                if (!res.IsError)
                {
                    productImages.Add(ProductImage.Create(res.Value.Url, productImages.Count, res.Value.PublicId));
                }
            }
        }

        var product = Product.Create(
            cmd.Name,
            cmd.Description,
            CategoryId.Create(cmd.CategoryId),
             cmd.Sku, cmd.StockQuantity,
             Price.CreateNew(cmd.UnitPrice, cmd.Currency),
             cmd.Discount,
              cmd.Brand,
              cmd.Barcode,
              cmd.Tags, "",
              productImages
            );
        await _productRepository.AddAsync(product);

        if (await _productRepository.Commit(cancellationToken) <= 0)
            return Errors.Product.ProductNotCreated;

        return product;

    }
}