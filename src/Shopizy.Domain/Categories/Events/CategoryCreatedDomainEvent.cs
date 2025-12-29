using Shopizy.Domain.Common.Models;

namespace Shopizy.Domain.Categories.Events;

public record CategoryCreatedDomainEvent(Category Category) : IDomainEvent;
