using Shopizy.Domain.LoyaltyAccounts;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Application.Common.Interfaces.Persistence;

public interface ILoyaltyAccountRepository
{
    Task<LoyaltyAccount?> GetByUserIdAsync(UserId userId);
    Task AddAsync(LoyaltyAccount account);
    void Update(LoyaltyAccount account);
}
