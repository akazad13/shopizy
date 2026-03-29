using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.ProductReviews.Events;
using Shopizy.SharedKernel.Application.Interfaces.Persistence;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.ProductReviews.Events;

public class ProductReviewDeletedDomainEventHandler(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork
) : IDomainEventHandler<ProductReviewDeletedDomainEvent>
{
    public async Task Handle(ProductReviewDeletedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var product = await productRepository.GetProductByIdForUpdateAsync(domainEvent.ProductId);
        if (product is null)
        {
            return;
        }

        product.RemoveReviewRating(domainEvent.Rating);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
