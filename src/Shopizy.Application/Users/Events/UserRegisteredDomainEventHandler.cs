using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Carts;
using Shopizy.Domain.Users.Events;
using Shopizy.SharedKernel.Application.Interfaces.Persistence;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Users.Events;

public class UserRegisteredDomainEventHandler(
    ICartRepository cartRepository,
    IUnitOfWork unitOfWork
) : IDomainEventHandler<UserRegisteredDomainEvent>
{
    private readonly ICartRepository _cartRepository = cartRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task Handle(
        UserRegisteredDomainEvent domainEvent,
        CancellationToken cancellationToken = default
    )
    {
        var cart = Cart.Create(domainEvent.User.Id);
        await _cartRepository.AddAsync(cart);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
