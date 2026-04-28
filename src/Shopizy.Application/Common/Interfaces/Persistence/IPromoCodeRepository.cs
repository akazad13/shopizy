using Shopizy.Domain.PromoCodes;
using Shopizy.Domain.PromoCodes.ValueObjects;

namespace Shopizy.Application.Common.Interfaces.Persistence;

public interface IPromoCodeRepository
{
    Task<IReadOnlyList<PromoCode>> GetPromoCodesAsync(int pageNumber, int pageSize);
    Task<PromoCode?> GetPromoCodeByIdAsync(PromoCodeId id);
    Task<PromoCode?> GetByCodeAsync(string code);
    Task AddAsync(PromoCode promoCode);
    void Update(PromoCode promoCode);
    void Remove(PromoCode promoCode);
}
