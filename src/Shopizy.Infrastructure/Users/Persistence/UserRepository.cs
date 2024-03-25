using Microsoft.EntityFrameworkCore;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Users;
using Shopizy.Infrastructure.Common.Persistence;

namespace Shopizy.Infrastructure.Users.Persistence;

public class UserRepository(AppDbContext _dbContext) : IUserRepository
{
    public Task<User?> GetUserByPhone(string phone)
    {
        return _dbContext.Users.SingleOrDefaultAsync(u => u.Phone == phone);
    }

    public async Task AddAsync(User user)
    {
        await _dbContext.Users.AddAsync(user);
    }

    public Task<int> Commit(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
