using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.Products.ValueObjects;
using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Domain.ProductReviews.Events;

public record ProductReviewDeletedDomainEvent(ProductId ProductId, Rating Rating) : IDomainEvent;
