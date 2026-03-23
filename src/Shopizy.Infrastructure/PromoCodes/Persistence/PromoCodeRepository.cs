using Microsoft.EntityFrameworkCore;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.PromoCodes;
using Shopizy.Domain.PromoCodes.ValueObjects;
using Shopizy.Infrastructure.Common.Persistence;

namespace Shopizy.Infrastructure.PromoCodes.Persistence;

public class PromoCodeRepository(AppDbContext dbContext) : IPromoCodeRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task<IReadOnlyList<PromoCode>> GetPromoCodesAsync()
    {
        return await _dbContext.PromoCodes.AsNoTracking().ToListAsync();
    }

    public Task<PromoCode?> GetPromoCodeByIdAsync(PromoCodeId id)
    {
        return _dbContext.PromoCodes.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task AddAsync(PromoCode promoCode)
    {
        await _dbContext.PromoCodes.AddAsync(promoCode);
    }

    public Task<PromoCode?> GetByCodeAsync(string code)
    {
        return _dbContext.PromoCodes.FirstOrDefaultAsync(pc => pc.Code == code);
    }

    public void Update(PromoCode promoCode)
    {
        _dbContext.PromoCodes.Update(promoCode);
    }

    public void Remove(PromoCode promoCode)
    {
        _dbContext.PromoCodes.Remove(promoCode);
    }
}
