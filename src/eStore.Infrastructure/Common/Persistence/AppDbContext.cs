using eStore.Domain.Common;
using eStore.Infrastructure.Common.Middleware;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace eStore.Infrastructure.Common.Persistence;
public class AppDbContext(DbContextOptions options, IHttpContextAccessor _httpContextAccessor, IPublisher _publisher) : DbContext(options)
{
    public async override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Get the domain events from the entity framework change tracker
        var domainEvents = ChangeTracker.Entries<Entity>()
            .SelectMany(entry => entry.Entity.PopDomainEvents())
            .ToList();

        if(IsUserWaitingOnline())
        {
            AddDomainEventsToOfflineProcessingQueue(domainEvents);
            return await base.SaveChangesAsync(cancellationToken);
        }

        // Publish all the domain event
        await PublishDomainEvents(domainEvents);
        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    private bool IsUserWaitingOnline() => _httpContextAccessor.HttpContext is not null;

    private async Task PublishDomainEvents(List<IDomainEvent> domainEvents)
    {
        foreach(var domainEvent in domainEvents)
        {
            await _publisher.Publish(domainEvent);
        }
    }

    private void AddDomainEventsToOfflineProcessingQueue(List<IDomainEvent> domainEvents)
    {
        // Get pending domain events from session
        Queue<IDomainEvent> domainEventsQueue = _httpContextAccessor.HttpContext!.Items.TryGetValue(EventualConsistencyMiddleware.DomainEventsKey, out var value) &&
            value is Queue<IDomainEvent> existingDomainEvents
            ? existingDomainEvents : new();

        // Add new domain event to the Queue
        domainEvents.ForEach(domainEventsQueue.Enqueue);

        // Update the session with newly added events
        _httpContextAccessor.HttpContext.Items[EventualConsistencyMiddleware.DomainEventsKey] = domainEventsQueue;
    }
}