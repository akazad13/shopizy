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

    public async Task Add(User user)
    {
        await _dbContext.Users.AddAsync(user);
    }

    public async Task<bool> Commit(CancellationToken cancellationToken)
{
    return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
}
}
