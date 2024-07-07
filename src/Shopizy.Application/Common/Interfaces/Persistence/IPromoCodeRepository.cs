using Shopizy.Domain.PromoCodes;
using Shopizy.Domain.PromoCodes.ValueObjects;

namespace Shopizy.Application.Common.Interfaces.Persistence;

public interface IPromoCodeRepository
{
    Task<List<PromoCode>> GetPromoCodesAsync();
    Task<PromoCode?> GetPromoCodeByIdAsync(PromoCodeId id);
    Task AddAsync(PromoCode promoCode);
    void Update(PromoCode promoCode);
    Task<int> Commit(CancellationToken cancellationToken);
}
