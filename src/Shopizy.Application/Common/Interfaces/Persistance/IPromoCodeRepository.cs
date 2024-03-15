using Shopizy.Domain.PromoCodes;
using Shopizy.Domain.PromoCodes.ValueObjects;

namespace shopizy.Application.Common.Interfaces.Persistance;

public interface IPromoCodeRepository
{
    Task<List<PromoCode>> GetPromoCodesAsync();
    Task<PromoCode?> GetPromoCodeByIdAsync(PromoCodeId id);
    Task AddAsync(PromoCode promoCode);
    void Update(PromoCode promoCode);
    Task<int> Commit(CancellationToken cancellationToken);
}
