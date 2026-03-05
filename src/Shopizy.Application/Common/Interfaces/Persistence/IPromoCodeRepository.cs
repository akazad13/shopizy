using System.Collections.Generic;
using System.Threading.Tasks;
using Shopizy.Domain.PromoCodes;
using Shopizy.Domain.PromoCodes.ValueObjects;

namespace Shopizy.Application.Common.Interfaces.Persistence;

public interface IPromoCodeRepository
{
    Task<IReadOnlyList<PromoCode>> GetPromoCodesAsync();
    Task<PromoCode?> GetPromoCodeByIdAsync(PromoCodeId id);
    Task AddAsync(PromoCode promoCode);
    void Update(PromoCode promoCode);
}
