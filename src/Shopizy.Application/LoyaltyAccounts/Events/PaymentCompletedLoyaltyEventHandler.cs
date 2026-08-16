using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.LoyaltyAccounts;
using Shopizy.Domain.Orders.Events;
using Shopizy.SharedKernel.Application.Interfaces.Persistence;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.LoyaltyAccounts.Events;

public class PaymentCompletedLoyaltyEventHandler(
    IOrderRepository orderRepository,
    ILoyaltyAccountRepository loyaltyAccountRepository,
    IUnitOfWork unitOfWork
) : IDomainEventHandler<PaymentCompletedDomainEvent>
{
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly ILoyaltyAccountRepository _loyaltyAccountRepository = loyaltyAccountRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task Handle(
        PaymentCompletedDomainEvent domainEvent,
        CancellationToken cancellationToken = default
    )
    {
        var order = await _orderRepository.GetOrderByIdAsync(domainEvent.OrderId);

        if (order is null)
        {
            return;
        }

        var pointsToEarn = (int)Math.Floor(order.GetTotal().Amount);

        if (pointsToEarn <= 0)
        {
            return;
        }

        var account = await _loyaltyAccountRepository.GetByUserIdAsync(domainEvent.UserId);

        if (account is null)
        {
            account = LoyaltyAccount.Create(domainEvent.UserId);
            await _loyaltyAccountRepository.AddAsync(account);
        }

        account.EarnPoints(pointsToEarn, $"Earned points for order {order.Id.Value}");

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
