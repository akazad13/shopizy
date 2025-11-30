using Microsoft.EntityFrameworkCore;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Users;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.Infrastructure.Common.Persistence;

namespace Shopizy.Infrastructure.Users.Persistence;

/// <summary>
/// Repository for managing user data persistence.
/// </summary>
public class UserRepository(AppDbContext dbContext) : IUserRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    /// <summary>
    /// Retrieves a user by their email address.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <returns>The user if found; otherwise, null.</returns>
    public Task<User?> GetUserByEmailAsync(string email)
    {
        return _dbContext.Users.SingleOrDefaultAsync(u => u.Email == email);
    }

    /// <summary>
    /// Retrieves a user by their unique identifier.
    /// </summary>
    /// <param name="id">The user's unique identifier.</param>
    /// <returns>The user if found; otherwise, null.</returns>
    public Task<User?> GetUserById(UserId id)
    {
        return _dbContext.Users.SingleOrDefaultAsync(u => u.Id == id);
    }

    /// <summary>
    /// Adds a new user to the database.
    /// </summary>
    /// <param name="user">The user to add.</param>
    public async Task AddAsync(User user)
    {
        await _dbContext.Users.AddAsync(user);
    }

    /// <summary>
    /// Updates an existing user in the database.
    /// </summary>
    /// <param name="user">The user to update.</param>
    public void Update(User user)
    {
        _dbContext.Users.Update(user);
    }

    /// <summary>
    /// Commits all pending changes to the database.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The number of state entries written to the database.</returns>
    public Task<int> Commit(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
