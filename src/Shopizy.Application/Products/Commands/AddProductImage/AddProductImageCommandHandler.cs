using System.Security.Cryptography.X509Certificates;
using MediatR;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Application.Common.Wrappers;
using Shopizy.Application.Products.Common;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Products.Entities;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Application.Products.Commands.AddProductImage;

public class AddProductImageCommandHandler(
    IProductRepository productRepository,
    IMediaUploader mediaUploader
) : IRequestHandler<AddProductImageCommand, IResult<ProductImage>>
{
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IMediaUploader _mediaUploader = mediaUploader;

    public async Task<IResult<ProductImage>> Handle(
        AddProductImageCommand cmd,
        CancellationToken cancellationToken
    )
    {
        if (cmd.File == null)
        {
            return Response<ProductImage>.ErrorResponse(
                [CustomErrors.Product.ProductImageNotUploaded]
            );
        }

        var product = await _productRepository.GetProductByIdAsync(ProductId.Create(cmd.ProductId));

        if (product is null)
        {
            return Response<ProductImage>.ErrorResponse([CustomErrors.Product.ProductNotFound]);
        }

        var photoUploadResult = await _mediaUploader.UploadPhotoAsync(cmd.File, cancellationToken);
        if (photoUploadResult.Succeeded())
        {
            var result = photoUploadResult.Match(x => x, x => new PhotoUploadResult("", ""));

            var productImage = ProductImage.Create(
                result.Url,
                product.ProductImages.Count,
                result.PublicId
            );
            product.AddProductImage(productImage);

            _productRepository.Update(product);

            if (await _productRepository.Commit(cancellationToken) <= 0)
            {
                return Response<ProductImage>.ErrorResponse(
                    [CustomErrors.Product.ProductImageNotAdded]
                );
            }

            return Response<ProductImage>.SuccessResponese(productImage);
        }

        return Response<ProductImage>.ErrorResponse(["Unable to add product image."]);
    }
}
