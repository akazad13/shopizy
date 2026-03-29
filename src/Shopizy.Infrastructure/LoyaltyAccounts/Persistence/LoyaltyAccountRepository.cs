using Microsoft.EntityFrameworkCore;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.LoyaltyAccounts;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.Infrastructure.Common.Persistence;

namespace Shopizy.Infrastructure.LoyaltyAccounts.Persistence;

public class LoyaltyAccountRepository(AppDbContext dbContext) : ILoyaltyAccountRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public Task<LoyaltyAccount?> GetByUserIdAsync(UserId userId)
    {
        return _dbContext.Set<LoyaltyAccount>()
            .FirstOrDefaultAsync(la => la.UserId == userId);
    }

    public async Task AddAsync(LoyaltyAccount account)
    {
        await _dbContext.Set<LoyaltyAccount>().AddAsync(account);
    }

    public void Update(LoyaltyAccount account)
    {
        _dbContext.Set<LoyaltyAccount>().Update(account);
    }
}
