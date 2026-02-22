using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Domain.Users.Events;

public record UserRegisteredDomainEvent(User User) : IDomainEvent;
