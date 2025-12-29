using MediatR;
using Microsoft.AspNetCore.Http;
using Shopizy.Domain.Common.Models;
using Shopizy.Infrastructure.Common.Persistence;

namespace Shopizy.Infrastructure.Common.Middleware;

public class EventualConsistencyMiddleware(RequestDelegate _next)
{
    public const string DomainEventsKey = "DomainEventsKey";

    public async Task InvokeAsync(HttpContext context, IPublisher publisher, AppDbContext dbContext)
    {
        Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction =
            await dbContext.Database.BeginTransactionAsync();
        await _next(context);

        await transaction.CommitAsync();

        try
        {
            if (
                context.Items.TryGetValue(DomainEventsKey, out object? value)
                && value is Queue<IDomainEvent> domainEvent
            )
            {
                while (domainEvent.TryDequeue(out IDomainEvent? nextEvent))
                {
                    await publisher.Publish(nextEvent);
                }
            }
        }
        catch (Exception)
        {
            // If publishing fails, data is already committed.
            // This is "Best Effort" consistency.
        }
        finally
        {
            await transaction.DisposeAsync();
        }
    }
}
