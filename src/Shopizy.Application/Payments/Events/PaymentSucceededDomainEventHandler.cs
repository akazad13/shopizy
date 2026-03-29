using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Payments.Events;
using Shopizy.SharedKernel.Application.Interfaces.Persistence;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Payments.Events;

public class PaymentSucceededDomainEventHandler(
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork
) : IDomainEventHandler<PaymentSucceededDomainEvent>
{
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task Handle(
        PaymentSucceededDomainEvent domainEvent,
        CancellationToken cancellationToken = default
    )
    {
        var order = await _orderRepository.GetOrderByIdAsync(domainEvent.OrderId);
        if (order is null)
        {
            return;
        }

        order.CompletePayment(domainEvent.CustomerId);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
