using Microsoft.EntityFrameworkCore;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Permissions;
using Shopizy.Domain.Permissions.ValueObjects;
using Shopizy.Infrastructure.Common.Persistence;

namespace Shopizy.Infrastructure.Permissions.Persistence;

public class PermissionRepository(AppDbContext dbContext) : IPermissionRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public Task<List<Permission>> GetAsync()
    {
        return _dbContext.Permissions.ToListAsync();
    }

    public Task<Permission?> GetByIdAsync(PermissionId id)
    {
        return _dbContext.Permissions.SingleOrDefaultAsync(p => p.Id == id);
    }

    public async Task AddAsync(Permission user)
    {
        await _dbContext.Permissions.AddAsync(user);
    }

    public void Update(Permission user)
    {
        _dbContext.Permissions.Update(user);
    }

    public Task<int> CommitAsync(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
