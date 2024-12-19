using Shopizy.Domain.Users;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Application.Common.Interfaces.Persistence;

public interface IUserRepository
{
    Task<User?> GetUserByPhoneAsync(string phone);
    Task<User?> GetUserById(UserId id);
    Task AddAsync(User user);
    void Update(User user);
    Task<int> Commit(CancellationToken cancellationToken);
}
