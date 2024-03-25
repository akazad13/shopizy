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

public class CreateProductCommandHandler(IProductRepository _productRepository, ICloudinaryMediaUploader _mediaUploader)
        : IRequestHandler<CreateProductCommand, ErrorOr<Product>>
{
    public async Task<ErrorOr<Product>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var productImages = new List<ProductImage>();
        if (request.Images != null)
        {
            foreach (var image in request.Images)
            {
                var res = await _mediaUploader.UploadPhotoAsync(image, cancellationToken);
                if (!res.IsError)
                {
                    productImages.Add(ProductImage.Create(res.Value.Url, productImages.Count, res.Value.PublicId));
                }
            }
        }

        var product = Product.Create(
            request.Name,
            request.Description,
            CategoryId.Create(request.CategoryId),
             request.Sku, request.StockQuantity,
             Price.CreateNew(request.UnitPrice, request.Currency),
             request.Discount,
              request.Brand,
              request.Barcode,
              request.Tags, "",
              productImages
            );
        await _productRepository.AddAsync(product);

        if (await _productRepository.Commit(cancellationToken) <= 0)
            return Errors.Product.ProductNotCreated;

        return product;

    }
}