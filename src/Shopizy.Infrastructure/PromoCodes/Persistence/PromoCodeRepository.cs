using Microsoft.EntityFrameworkCore;
using Shopizy.Application.Common.Interfaces.Persistance;
using Shopizy.Domain.PromoCodes;
using Shopizy.Domain.PromoCodes.ValueObjects;
using Shopizy.Infrastructure.Common.Persistence;

namespace Shopizy.Infrastructure.PromoCodes.Persistence;

public class PromoCodeRepository(AppDbContext _dbContext) : IPromoCodeRepository
{
    public Task<List<PromoCode>> GetPromoCodesAsync()
    {
        return _dbContext.PromoCodes.AsNoTracking().ToListAsync();
    }
    public Task<PromoCode?> GetPromoCodeByIdAsync(PromoCodeId id)
    {
        return _dbContext.PromoCodes.FirstOrDefaultAsync(c => c.Id == id);
    }
    public async Task AddAsync(PromoCode promoCode)
    {
        await _dbContext.PromoCodes.AddAsync(promoCode);
    }
    public void Update(PromoCode promoCode)
    {
        _dbContext.Update(promoCode);
    }

    public Task<int> Commit(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}