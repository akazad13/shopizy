using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Domain.Users.Events;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Users.Events;

public class UserWelcomeEmailDomainEventHandler(IEmailService emailService)
    : IDomainEventHandler<UserRegisteredDomainEvent>
{
    public async Task Handle(
        UserRegisteredDomainEvent domainEvent,
        CancellationToken cancellationToken = default
    )
    {
        var user = domainEvent.User;

        await emailService.SendAsync(
            to: user.Email,
            subject: "Welcome to Shopizy!",
            body: $"Hi {user.FirstName},\n\nWelcome to Shopizy! Your account has been created successfully.\n\nHappy shopping!",
            cancellationToken: cancellationToken
        );
    }
}
