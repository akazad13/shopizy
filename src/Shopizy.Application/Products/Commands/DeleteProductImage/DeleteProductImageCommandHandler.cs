using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Products.ValueObjects;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Products.Commands.DeleteProductImage;

public class DeleteProductImageCommandHandler(
    IProductRepository productRepository,
    IMediaUploader mediaUploader
) : ICommandHandler<DeleteProductImageCommand, ErrorOr<Success>>
{
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IMediaUploader _mediaUploader = mediaUploader;

    public async Task<ErrorOr<Success>> Handle(
        DeleteProductImageCommand cmd,
        CancellationToken cancellationToken
    )
    {
        var product = await _productRepository.GetProductByIdForUpdateAsync(
            ProductId.Create(cmd.ProductId)
        );

        if (product is null)
        {
            return (Error)CustomErrors.Product.ProductNotFound;
        }

        var prodImage = product.ProductImages.FirstOrDefault(pi =>
            pi.Id == ProductImageId.Create(cmd.ImageId)
        );

        if (prodImage is null)
        {
            return (Error)CustomErrors.Product.ProductImageNotFound;
        }

        var res = await _mediaUploader.DeletePhotoAsync(prodImage.PublicId);

        if (!res.IsError)
        {
            product.RemoveProductImage(prodImage);

            return Result.Success;
        }
        return res.Errors;
    }
}
