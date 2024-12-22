using Shopizy.Domain.Users;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Application.Common.Interfaces.Persistence;

public interface IUserRepository
{
    Task<User?> GetUserByEmailAsync(string email);
    Task<User?> GetUserById(UserId id);
    Task AddAsync(User user);
    void Update(User user);
    Task<int> Commit(CancellationToken cancellationToken);
}
