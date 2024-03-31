using ErrorOr;
using MediatR;
using Shopizy.Application.Common.Interfaces.Persistance;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Application.Products.Commands.DeleteProduct;

public class DeleteProductCommandHandler(IProductRepository _productRepository, IMediaUploader _mediaUploader)
        : IRequestHandler<DeleteProductCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(DeleteProductCommand cmd, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetProductByIdAsync(ProductId.Create(cmd.ProductId));

        if(product is null)
            return CustomErrors.Product.ProductNotFound;

        // Delete product image from media

        _productRepository.Remove(product);

        if (await _productRepository.Commit(cancellationToken) <= 0)
            return CustomErrors.Product.ProductNotDeleted;

        return Result.Success;

    }
}