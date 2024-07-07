using ErrorOr;
using MediatR;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Products.Entities;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Application.Products.Commands.AddProductImage;

public class AddProductImageCommandHandler(IProductRepository productRepository, IMediaUploader mediaUploader)
        : IRequestHandler<AddProductImageCommand, ErrorOr<ProductImage>>
{
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IMediaUploader _mediaUploader = mediaUploader;
    
    public async Task<ErrorOr<ProductImage>> Handle(AddProductImageCommand cmd, CancellationToken cancellationToken)
    {
        if(cmd.File is null)
            return CustomErrors.Product.ProductImageNotUploaded;
        
        var product = await _productRepository.GetProductByIdAsync(ProductId.Create(cmd.ProductId));

        if(product is null)
            return CustomErrors.Product.ProductNotFound;
       
        ProductImage productImage;

        var res = await _mediaUploader.UploadPhotoAsync(cmd.File, cancellationToken);
        if (!res.IsError)
        {
            productImage = ProductImage.Create(res.Value.Url, product.ProductImages.Count, res.Value.PublicId);
            product.AddProductImage(productImage);
            
            _productRepository.Update(product);

            if (await _productRepository.Commit(cancellationToken) <= 0)
                return CustomErrors.Product.ProductImageNotAdded;
            return productImage;
        }
        return res.Errors;

    }
}