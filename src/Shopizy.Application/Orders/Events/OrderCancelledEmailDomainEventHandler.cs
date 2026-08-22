using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Domain.Orders.Events;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Orders.Events;

public class OrderCancelledEmailDomainEventHandler(
    IUserRepository userRepository,
    IEmailService emailService
) : IDomainEventHandler<OrderCancelledDomainEvent>
{
    public async Task Handle(
        OrderCancelledDomainEvent domainEvent,
        CancellationToken cancellationToken = default
    )
    {
        var order = domainEvent.Order;
        var user = await userRepository.GetUserByIdAsync(order.UserId);
        if (user is null)
        {
            return;
        }

        await emailService.SendAsync(
            to: user.Email,
            subject: $"Order #{order.Id.Value} Cancelled",
            body: $"Hi {user.FirstName},\n\nYour order #{order.Id.Value} has been cancelled. Reason: {order.CancellationReason ?? "Customer request"}.\n\nIf you have any questions, please contact customer support.",
            cancellationToken: cancellationToken
        );
    }
}
