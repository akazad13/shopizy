using Shopizy.Domain.Permissions;
using Shopizy.Domain.Permissions.ValueObjects;

namespace Shopizy.Application.Common.Interfaces.Persistence;

public interface IPermissionRepository
{
    Task<List<Permission>> GetAsync();
    Task<Permission?> GetById(PermissionId id);
    Task AddAsync(Permission user);
    void Update(Permission user);
    Task<int> Commit(CancellationToken cancellationToken);
}
