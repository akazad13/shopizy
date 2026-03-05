using Shopizy.SharedKernel.Application.Messaging;
namespace Shopizy.Application.Common.Interfaces.Persistence;

public interface IAppDbContext
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}

