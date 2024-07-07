using ErrorOr;
using MediatR;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Domain.Products.ValueObjects;
using Shopizy.Application.Products.Commands.DeleteProductImage;

namespace Shopizy.Application.Products.Commands.DeleteProduct;

public class DeleteProductImageCommandHandler(IProductRepository productRepository, IMediaUploader mediaUploader)
        : IRequestHandler<DeleteProductImageCommand, ErrorOr<Success>>
{
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IMediaUploader _mediaUploader = mediaUploader;

    public async Task<ErrorOr<Success>> Handle(DeleteProductImageCommand cmd, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetProductByIdAsync(ProductId.Create(cmd.ProductId));

        if(product is null)
            return CustomErrors.Product.ProductNotFound;

        var prodImage = product.ProductImages.FirstOrDefault(pi => pi.Id ==  ProductImageId.Create(cmd.ImageId));

        if(prodImage is null)
            return CustomErrors.Product.ProductImageNotFound;

        var res = await _mediaUploader.DeletePhotoAsync(prodImage.PublicId);

        if (!res.IsError)
        {
            product.RemoveProductImage(prodImage);
            
            _productRepository.Update(product);

            if (await _productRepository.Commit(cancellationToken) <= 0)
                return CustomErrors.Product.ProductImageNotAdded;
             return Result.Success;
        }
        return res.Errors;
    }
}