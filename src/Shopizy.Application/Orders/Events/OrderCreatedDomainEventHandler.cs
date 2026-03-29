using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Orders.Events;
using Shopizy.SharedKernel.Application.Interfaces.Persistence;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Orders.Events;

public class OrderCreatedDomainEventHandler(
    ICartRepository cartRepository,
    IProductRepository productRepository,
    IUnitOfWork unitOfWork
) : IDomainEventHandler<OrderCreatedDomainEvent>
{
    public async Task Handle(
        OrderCreatedDomainEvent domainEvent,
        CancellationToken cancellationToken = default
    )
    {
        var productIds = domainEvent.Order.OrderItems.Select(i => i.ProductId).ToList();
        var products = await productRepository.GetProductsByIdsForUpdateAsync(productIds);

        foreach (var item in domainEvent.Order.OrderItems)
        {
            var product = products.FirstOrDefault(p => p.Id == item.ProductId);
            if (product is not null)
            {
                product.ReduceStock(item.Quantity);
                productRepository.Update(product);
            }
        }

        var cart = await cartRepository.GetCartByUserIdForUpdateAsync(domainEvent.Order.UserId);
        if (cart is not null && cart.CartItems.Count > 0)
        {
            cart.Clear();
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
