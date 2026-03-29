using ErrorOr;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Application.Products.Commands.DeleteProduct;

public class DeleteProductCommandHandler(
    IProductRepository productRepository,
    IMediaUploader mediaUploader
) : ICommandHandler<DeleteProductCommand, ErrorOr<Success>>
{
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IMediaUploader _mediaUploader = mediaUploader;

    public async Task<ErrorOr<Success>> Handle(
        DeleteProductCommand cmd,
        CancellationToken cancellationToken
    )
    {
        Domain.Products.Product? product = await _productRepository.GetProductByIdAsync(
            ProductId.Create(cmd.ProductId)
        );

        if (product is null)
        {
            return (Error)CustomErrors.Product.ProductNotFound;
        }

        foreach (var image in product.ProductImages)
        {
            await _mediaUploader.DeletePhotoAsync(image.PublicId);
        }

        _productRepository.Remove(product);

        return Result.Success;
    }
}
