using ErrorOr;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Application.Products.Commands.BulkUpdateProductStatus;

public class BulkUpdateProductStatusCommandHandler(IProductRepository productRepository)
    : ICommandHandler<BulkUpdateProductStatusCommand, ErrorOr<Success>>
{
    private readonly IProductRepository _productRepository = productRepository;

    public async Task<ErrorOr<Success>> Handle(BulkUpdateProductStatusCommand request, CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetProductsByIdsAsync(
            request.ProductIds.Select(ProductId.Create).ToList());

        foreach (var product in products)
        {
            product.SetIsActive(request.IsActive);
        }

        foreach (var product in products)
        {
            _productRepository.Update(product);
        }

        return Result.Success;
    }
}
