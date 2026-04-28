using ErrorOr;
using Shopizy.Domain.GiftCards;
using Shopizy.SharedKernel.Application.Caching;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.GiftCards.Queries.GetGiftCards;

public record GetGiftCardsQuery(int PageNumber, int PageSize)
    : IQuery<ErrorOr<IReadOnlyList<GiftCard>>>,
        ICachableRequest
{
    public string CacheKey => $"gift-cards-{PageNumber}-{PageSize}";
    public TimeSpan? Expiration => TimeSpan.FromMinutes(30);
}
