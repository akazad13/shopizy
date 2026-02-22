using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Domain.Categories.Events;

public record CategoryUpdatedDomainEvent(Category Category) : IDomainEvent;
