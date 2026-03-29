using ErrorOr;
using Shopizy.Domain.PromoCodes;
using Shopizy.SharedKernel.Application.Caching;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.PromoCodes.Queries.GetPromoCodes;

public record GetPromoCodesQuery(int PageNumber, int PageSize) : IQuery<ErrorOr<List<PromoCode>>>, ICachableRequest
{
    public string CacheKey => $"promo-codes-{PageNumber}-{PageSize}";
    public TimeSpan? Expiration => TimeSpan.FromMinutes(30);
}
