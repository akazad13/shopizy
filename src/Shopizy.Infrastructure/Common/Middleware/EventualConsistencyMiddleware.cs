using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Shopizy.SharedKernel.Domain.Models;
using Shopizy.Infrastructure.Common.Persistence;

namespace Shopizy.Infrastructure.Common.Middleware;

public class EventualConsistencyMiddleware(RequestDelegate _next, ILogger<EventualConsistencyMiddleware> logger)
{
    private readonly ILogger<EventualConsistencyMiddleware> _logger = logger;
    public const string DomainEventsKey = "DomainEventsKey";

    public async Task InvokeAsync(HttpContext context, IPublisher publisher, AppDbContext dbContext)
    {
        if (HttpMethods.IsGet(context.Request.Method))
        {
            await _next(context);
            return;
        }

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
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while publishing domain events.");
            // If publishing fails, data is already committed.
            // This is "Best Effort" consistency.
        }
        finally
        {
            await transaction.DisposeAsync();
        }
    }
}
