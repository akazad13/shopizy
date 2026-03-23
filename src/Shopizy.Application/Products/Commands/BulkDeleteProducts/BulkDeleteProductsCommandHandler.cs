using ErrorOr;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Application.Products.Commands.BulkDeleteProducts;

public class BulkDeleteProductsCommandHandler(IProductRepository productRepository)
    : ICommandHandler<BulkDeleteProductsCommand, ErrorOr<Deleted>>
{
    private readonly IProductRepository _productRepository = productRepository;

    public async Task<ErrorOr<Deleted>> Handle(BulkDeleteProductsCommand request, CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetProductsByIdsAsync(
            request.ProductIds.Select(ProductId.Create).ToList());

        if (products.Count == 0)
        {
            return CustomErrors.Product.ProductNotFound;
        }

        _productRepository.RemoveRange(products.ToList());

        return Result.Deleted;
    }
}
