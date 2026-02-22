using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.SharedKernel.Application.Interfaces.Persistence;
using Shopizy.Domain.Carts;
using Shopizy.Domain.Categories;
using Shopizy.SharedKernel.Domain.Models;
using Shopizy.Domain.Orders;
using Shopizy.Domain.Payments;
using Shopizy.Domain.Permissions;
using Shopizy.Domain.ProductReviews;
using Shopizy.Domain.Products;
using Shopizy.Domain.PromoCodes;
using Shopizy.Domain.Users;
using Shopizy.Infrastructure.Common.Middleware;

namespace Shopizy.Infrastructure.Common.Persistence;

/// <summary>
/// The application's database context for Entity Framework Core.
/// Handles domain events and eventual consistency.
/// </summary>
public class AppDbContext(
    DbContextOptions options,
    IHttpContextAccessor _httpContextAccessor
) : DbContext(options), IAppDbContext, IUnitOfWork
{
    /// <summary>
    /// Gets or sets the categories DbSet.
    /// </summary>
    public DbSet<Category> Categories { get; set; }
    
    /// <summary>
    /// Gets or sets the carts DbSet.
    /// </summary>
    public DbSet<Cart> Carts { get; set; }
    
    /// <summary>
    /// Gets or sets the orders DbSet.
    /// </summary>
    public DbSet<Order> Orders { get; set; }
    
    /// <summary>
    /// Gets or sets the payments DbSet.
    /// </summary>
    public DbSet<Payment> Payments { get; set; }
    
    /// <summary>
    /// Gets or sets the products DbSet.
    /// </summary>
    public DbSet<Product> Products { get; set; }
    
    /// <summary>
    /// Gets or sets the product reviews DbSet.
    /// </summary>
    public DbSet<ProductReview> ProductReviews { get; set; }
    
    /// <summary>
    /// Gets or sets the promo codes DbSet.
    /// </summary>
    public DbSet<PromoCode> PromoCodes { get; set; }
    
    /// <summary>
    /// Gets or sets the users DbSet.
    /// </summary>
    public DbSet<User> Users { get; set; }
    
    /// <summary>
    /// Gets or sets the permissions DbSet.
    /// </summary>
    public DbSet<Permission> Permissions { get; set; }

    /// <summary>
    /// Saves all changes made in this context to the database.
    /// Handles domain event publishing with eventual consistency support via Best Effort (Offline Queue).
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The number of state entries written to the database.</returns>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Get the domain events from the entity framework change tracker
        var domainEvents = ChangeTracker
            .Entries<IHasDomainEvents>()
            .SelectMany(entry => entry.Entity.PopDomainEvents())
            .ToList();

        if (IsUserWaitingOnline())
        {
            AddDomainEventsToOfflineProcessingQueue(domainEvents);
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Ignore<List<IDomainEvent>>()
            .ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        
        // Hack for integration tests: remove SQL Server specific column types when using PostgreSQL
        if (Database.ProviderName != "Microsoft.EntityFrameworkCore.SqlServer")
        {
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entity.GetProperties())
                {
                    if (property.GetColumnType() == "smalldatetime")
                    {
                        property.SetColumnType(null);
                    }
                }
            }
        }

        base.OnModelCreating(modelBuilder);
    }

    private bool IsUserWaitingOnline() => _httpContextAccessor.HttpContext is not null;

    private void AddDomainEventsToOfflineProcessingQueue(List<IDomainEvent> domainEvents)
    {
        // Get pending domain events from session
        Queue<IDomainEvent> domainEventsQueue =
            _httpContextAccessor.HttpContext!.Items.TryGetValue(
                EventualConsistencyMiddleware.DomainEventsKey,
                out object? value
            ) && value is Queue<IDomainEvent> existingDomainEvents
                ? existingDomainEvents
                : new();

        // Add new domain event to the Queue
        domainEvents.ForEach(domainEventsQueue.Enqueue);

        // Update the session with newly added events
        _httpContextAccessor.HttpContext.Items[EventualConsistencyMiddleware.DomainEventsKey] =
            domainEventsQueue;
    }
}
