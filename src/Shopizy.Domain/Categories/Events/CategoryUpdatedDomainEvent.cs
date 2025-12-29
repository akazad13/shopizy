using Shopizy.Domain.Common.Models;

namespace Shopizy.Domain.Categories.Events;

public record CategoryUpdatedDomainEvent(Category Category) : IDomainEvent;
