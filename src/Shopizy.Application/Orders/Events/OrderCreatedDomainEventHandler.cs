using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Orders.Events;
using Shopizy.SharedKernel.Application.Interfaces.Persistence;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Orders.Events;

public class OrderCreatedDomainEventHandler(
    ICartRepository cartRepository,
    IUnitOfWork unitOfWork
) : IDomainEventHandler<OrderCreatedDomainEvent>
{
    public async Task Handle(
        OrderCreatedDomainEvent domainEvent,
        CancellationToken cancellationToken = default
    )
    {
        var cart = await cartRepository.GetCartByUserIdForUpdateAsync(domainEvent.Order.UserId);

        if (cart is null || cart.CartItems.Count == 0)
        {
            return;
        }

        cart.Clear();
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
