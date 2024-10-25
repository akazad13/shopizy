using MediatR;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Application.Common.Wrappers;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Application.Products.Commands.DeleteProductImage;

public class DeleteProductImageCommandHandler(
    IProductRepository productRepository,
    IMediaUploader mediaUploader
) : IRequestHandler<DeleteProductImageCommand, IResult<GenericResponse>>
{
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IMediaUploader _mediaUploader = mediaUploader;

    public async Task<IResult<GenericResponse>> Handle(
        DeleteProductImageCommand cmd,
        CancellationToken cancellationToken
    )
    {
        var product = await _productRepository.GetProductByIdAsync(ProductId.Create(cmd.ProductId));

        if (product is null)
        {
            return Response<GenericResponse>.ErrorResponse([CustomErrors.Product.ProductNotFound]);
        }

        var prodImage = product.ProductImages.FirstOrDefault(pi =>
            pi.Id == ProductImageId.Create(cmd.ImageId)
        );

        if (prodImage is null)
        {
            return Response<GenericResponse>.ErrorResponse(
                [CustomErrors.Product.ProductImageNotFound]
            );
        }

        var res = await _mediaUploader.DeletePhotoAsync(prodImage.PublicId);

        if (res.Succeeded)
        {
            product.RemoveProductImage(prodImage);

            _productRepository.Update(product);

            if (await _productRepository.Commit(cancellationToken) <= 0)
            {
                return Response<GenericResponse>.ErrorResponse(
                    [CustomErrors.Product.ProductImageNotAdded]
                );
            }

            return Response<GenericResponse>.SuccessResponese("Delete product image successfully.");
        }
        return Response<GenericResponse>.ErrorResponse(res.Errors);
    }
}
