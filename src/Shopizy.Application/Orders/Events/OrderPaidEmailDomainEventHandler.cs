using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Domain.Orders.Events;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Orders.Events;

public class OrderPaidEmailDomainEventHandler(
    IUserRepository userRepository,
    IEmailService emailService
) : IDomainEventHandler<PaymentCompletedDomainEvent>
{
    public async Task Handle(
        PaymentCompletedDomainEvent domainEvent,
        CancellationToken cancellationToken = default
    )
    {
        var user = await userRepository.GetUserByIdAsync(domainEvent.UserId);
        if (user is null)
        {
            return;
        }

        await emailService.SendAsync(
            to: user.Email,
            subject: $"Payment Received for Order #{domainEvent.OrderId.Value}",
            body: $"Hi {user.FirstName},\n\nWe have received payment for your order #{domainEvent.OrderId.Value}. We are currently processing your shipment.\n\nThank you for shopping with Shopizy!",
            cancellationToken: cancellationToken
        );
    }
}
