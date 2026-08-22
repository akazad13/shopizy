using Microsoft.Extensions.Logging;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Domain.Products.Events;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Products.Events;

/// <summary>
/// Domain event handler that notifies customers who have a discounted product in their wishlist.
/// </summary>
public sealed class ProductPriceDroppedDomainEventHandler(
    IWishlistRepository wishlistRepository,
    IUserRepository userRepository,
    IEmailService emailService,
    ILogger<ProductPriceDroppedDomainEventHandler> logger
) : IDomainEventHandler<ProductPriceDroppedDomainEvent>
{
    private static readonly Action<ILogger, string, Guid, Exception?> s_sendPriceDropFailed =
        LoggerMessage.Define<string, Guid>(
            LogLevel.Error,
            new EventId(1, nameof(ProductPriceDroppedDomainEventHandler)),
            "Failed to send price drop notification to {Email} for product {ProductId}."
        );

    private readonly IWishlistRepository _wishlistRepository = wishlistRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IEmailService _emailService = emailService;
    private readonly ILogger<ProductPriceDroppedDomainEventHandler> _logger = logger;

    public async Task Handle(
        ProductPriceDroppedDomainEvent domainEvent,
        CancellationToken cancellationToken = default
    )
    {
        var product = domainEvent.Product;
        var wishlists = await _wishlistRepository.GetWishlistsByProductIdAsync(
            product.Id,
            cancellationToken
        );

        if (wishlists.Count == 0)
        {
            return;
        }

        var distinctUserIds = wishlists.Select(w => w.UserId).Distinct().ToList();

        foreach (var userId in distinctUserIds)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user is null || string.IsNullOrWhiteSpace(user.Email))
            {
                continue;
            }

            var subject = $"Price Drop Alert: {product.Name} is now on sale!";
            var body =
                $"Hello {user.FirstName},\n\nGreat news! \"{product.Name}\" on your wishlist has dropped in price from {domainEvent.OldEffectivePrice:C} to {domainEvent.NewEffectivePrice:C}.\n\nVisit Shopizy to grab it while supplies last!";

            try
            {
                await _emailService.SendAsync(user.Email, subject, body, cancellationToken);
            }
            catch (Exception ex)
            {
                s_sendPriceDropFailed(_logger, user.Email, product.Id.Value, ex);
            }
        }
    }
}
