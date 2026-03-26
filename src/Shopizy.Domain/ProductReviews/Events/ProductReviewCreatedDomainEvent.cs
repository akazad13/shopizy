using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.Products.ValueObjects;
using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Domain.ProductReviews.Events;

public record ProductReviewCreatedDomainEvent(ProductId ProductId, Rating Rating) : IDomainEvent;
