using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shopizy.Domain.AuditLogs;
using Shopizy.Domain.Brands;
using Shopizy.Domain.Carts;
using Shopizy.Domain.Categories;
using Shopizy.Domain.GiftCards;
using Shopizy.Domain.LoyaltyAccounts;
using Shopizy.Domain.Orders;
using Shopizy.Domain.Payments;
using Shopizy.Domain.Permissions;
using Shopizy.Domain.ProductQuestions;
using Shopizy.Domain.ProductReviews;
using Shopizy.Domain.Products;
using Shopizy.Domain.PromoCodes;
using Shopizy.Domain.Users;
using Shopizy.Domain.Wishlists;
using Shopizy.Infrastructure.Common.Middleware;
using Shopizy.Infrastructure.Outbox;
using Shopizy.SharedKernel.Application.Interfaces.Persistence;
using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Infrastructure.Common.Persistence;

/// <summary>
/// The application's database context for Entity Framework Core.
/// Handles domain events and eventual consistency.
/// </summary>
public class AppDbContext(DbContextOptions options, IHttpContextAccessor _httpContextAccessor)
    : DbContext(options),
        IUnitOfWork
{
    /// <summary>
    /// Gets or sets the brands DbSet.
    /// </summary>
    public DbSet<Brand> Brands { get; set; }

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
    /// Gets or sets the wishlists DbSet.
    /// </summary>
    public DbSet<Wishlist> Wishlists { get; set; }

    /// <summary>
    /// Gets or sets the loyalty accounts DbSet.
    /// </summary>
    public DbSet<LoyaltyAccount> LoyaltyAccounts { get; set; }

    /// <summary>
    /// Gets or sets the gift cards DbSet.
    /// </summary>
    public DbSet<GiftCard> GiftCards { get; set; }

    /// <summary>
    /// Gets or sets the product questions DbSet.
    /// </summary>
    public DbSet<ProductQuestion> ProductQuestions { get; set; }

    /// <summary>
    /// Gets or sets the audit logs DbSet.
    /// </summary>
    public DbSet<AuditLog> AuditLogs { get; set; }

    /// <summary>
    /// Gets or sets the outbox messages DbSet.
    /// </summary>
    public DbSet<OutboxMessage> OutboxMessages { get; set; }

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

        if (IsUserWaitingOnline() && domainEvents.Count > 0)
        {
            var outboxMessages = WriteEventsToOutbox(domainEvents);
            AddDomainEventsToOfflineProcessingQueue(domainEvents, outboxMessages);
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Creates <see cref="OutboxMessage"/> rows for each domain event so they are
    /// committed atomically with the aggregate change.
    /// </summary>
    private List<OutboxMessage> WriteEventsToOutbox(List<IDomainEvent> domainEvents)
    {
        var messages = domainEvents
            .Select(e => new OutboxMessage
            {
                Id = Guid.NewGuid(),
                OccurredOn = DateTime.UtcNow,
                Type = e.GetType().AssemblyQualifiedName!,
                Content = JsonSerializer.Serialize(e, e.GetType()),
            })
            .ToList();

        OutboxMessages.AddRange(messages);
        return messages;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Ignore<List<IDomainEvent>>()
            .ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            var rowVersionProp = entity.FindProperty("RowVersion");
            if (rowVersionProp?.ClrType == typeof(byte[]))
            {
                rowVersionProp.IsConcurrencyToken = true;
                rowVersionProp.ValueGenerated = Microsoft
                    .EntityFrameworkCore
                    .Metadata
                    .ValueGenerated
                    .OnAddOrUpdate;
                rowVersionProp.SetColumnType("rowversion");
            }
        }

        base.OnModelCreating(modelBuilder);
    }

    private bool IsUserWaitingOnline() => _httpContextAccessor.HttpContext is not null;

    private void AddDomainEventsToOfflineProcessingQueue(
        List<IDomainEvent> domainEvents,
        List<OutboxMessage> outboxMessages
    )
    {
        // Get pending domain events from session (carries outbox IDs for post-dispatch marking)
        Queue<(IDomainEvent Event, Guid OutboxId)> queue =
            _httpContextAccessor.HttpContext!.Items.TryGetValue(
                EventualConsistencyMiddleware.DomainEventsKey,
                out object? value
            ) && value is Queue<(IDomainEvent, Guid)> existing
                ? existing
                : new();

        for (int i = 0; i < domainEvents.Count; i++)
        {
            queue.Enqueue((domainEvents[i], outboxMessages[i].Id));
        }

        _httpContextAccessor.HttpContext.Items[EventualConsistencyMiddleware.DomainEventsKey] =
            queue;
    }
}
