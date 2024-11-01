using MediatR;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Application.Common.Wrappers;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Application.Products.Commands.DeleteProduct;

public class DeleteProductCommandHandler(
    IProductRepository productRepository,
    IMediaUploader mediaUploader
) : IRequestHandler<DeleteProductCommand, IResult<GenericResponse>>
{
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IMediaUploader _mediaUploader = mediaUploader;

    public async Task<IResult<GenericResponse>> Handle(
        DeleteProductCommand cmd,
        CancellationToken cancellationToken
    )
    {
        Domain.Products.Product? product = await _productRepository.GetProductByIdAsync(
            ProductId.Create(cmd.ProductId)
        );

        if (product is null)
        {
            return Response<GenericResponse>.ErrorResponse([CustomErrors.Product.ProductNotFound]);
        }

        // Delete product image from media

        _productRepository.Remove(product);

        if (await _productRepository.Commit(cancellationToken) <= 0)
        {
            return Response<GenericResponse>.ErrorResponse([CustomErrors.Product.ProductNotFound]);
        }
        return Response<GenericResponse>.SuccessResponese("Delete product successfully.");
    }
}
