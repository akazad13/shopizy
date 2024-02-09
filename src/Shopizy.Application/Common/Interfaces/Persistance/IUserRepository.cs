using Shopizy.Domain.Users;

namespace Shopizy.Application.Common.Interfaces.Persistence;

public interface IUserRepository
{
    Task<User?> GetUserByPhone(string phone);
    Task Add(User user);
    Task<bool> Commit(CancellationToken cancellationToken);
}
