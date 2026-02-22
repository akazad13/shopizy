using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Domain.Categories.Events;

public record CategoryCreatedDomainEvent(Category Category) : IDomainEvent;
