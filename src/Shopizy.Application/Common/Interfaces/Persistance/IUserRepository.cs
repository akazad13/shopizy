using Shopizy.Domain.Users;

namespace Shopizy.Application.Common.Interfaces.Persistence;

public interface IUserRepository
{
    Task<User?> GetUserByPhone(string phone);
    Task AddAsync(User user);
    Task<int> Commit(CancellationToken cancellationToken);
}
