using Shopizy.Domain.Users;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Application.Common.Interfaces.Persistence;

public interface IUserRepository
{
    Task<User?> GetUserByEmailAsync(string email);
    Task<User?> GetUserByIdAsync(UserId id);
    Task<User?> GetUserByResetTokenAsync(string token);
    Task<int> GetTotalUsersCountAsync();
    Task<IReadOnlyList<User>> ListUsersAsync(int pageNumber, int pageSize);
    Task AddAsync(User user);
    void Update(User user);
}

