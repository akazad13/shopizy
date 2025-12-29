using Shopizy.Domain.Common.Models;

namespace Shopizy.Domain.Users.Events;

public record UserRegisteredDomainEvent(User User) : IDomainEvent;
