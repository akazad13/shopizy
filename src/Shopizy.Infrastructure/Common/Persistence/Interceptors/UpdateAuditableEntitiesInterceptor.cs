using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Infrastructure.Common.Persistence.Interceptors;

/// <summary>
/// Interceptor for automatically setting <see cref="IAuditable.CreatedOn"/> and <see cref="IAuditable.ModifiedOn"/> properties.
/// </summary>
public class UpdateAuditableEntitiesInterceptor(IDateTimeProvider dateTimeProvider) : SaveChangesInterceptor
{
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateEntities(DbContext? context)
    {
        if (context == null) return;

        foreach (var entry in context.ChangeTracker.Entries<IAuditable>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Property(nameof(IAuditable.CreatedOn)).CurrentValue = _dateTimeProvider.UtcNow;
            }

            if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
            {
                entry.Property(nameof(IAuditable.ModifiedOn)).CurrentValue = _dateTimeProvider.UtcNow;
            }
        }
    }
}
