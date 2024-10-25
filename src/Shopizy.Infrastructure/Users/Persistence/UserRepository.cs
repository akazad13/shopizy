using Microsoft.EntityFrameworkCore;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Users;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.Infrastructure.Common.Persistence;

namespace Shopizy.Infrastructure.Users.Persistence;

public class UserRepository(AppDbContext dbContext) : IUserRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public Task<User?> GetUserByPhone(string phone)
    {
        return _dbContext.Users.SingleOrDefaultAsync(u => u.Phone == phone);
    }

    public Task<User?> GetUserById(UserId id)
    {
        return _dbContext.Users.SingleOrDefaultAsync(u => u.Id == id);
    }

    public async Task AddAsync(User user)
    {
        await _dbContext.Users.AddAsync(user);
    }

    public void Update(User user)
    {
        _dbContext.Users.Update(user);
    }

    public Task<int> Commit(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
