using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Orders.Events;
using Shopizy.SharedKernel.Application.Interfaces.Persistence;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Payments.Events;

public class PaymentCompletedDomainEventHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork
) : IDomainEventHandler<PaymentCompletedDomainEvent>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task Handle(
        PaymentCompletedDomainEvent domainEvent,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrEmpty(domainEvent.CustomerId))
        {
            return;
        }

        var user = await _userRepository.GetUserByIdAsync(domainEvent.UserId);

        if (user is null)
        {
            return;
        }

        user.UpdateCustomerId(domainEvent.CustomerId);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
