using Microsoft.Extensions.Logging;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Domain.Products.Events;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Products.Events;

/// <summary>
/// Domain event handler that notifies customers who have a restocked product in their wishlist.
/// </summary>
public sealed class ProductBackInStockDomainEventHandler(
    IWishlistRepository wishlistRepository,
    IUserRepository userRepository,
    IEmailService emailService,
    ILogger<ProductBackInStockDomainEventHandler> logger
) : IDomainEventHandler<ProductBackInStockDomainEvent>
{
    private static readonly Action<ILogger, string, Guid, Exception?> s_sendBackInStockFailed =
        LoggerMessage.Define<string, Guid>(
            LogLevel.Error,
            new EventId(1, nameof(ProductBackInStockDomainEventHandler)),
            "Failed to send back in stock notification to {Email} for product {ProductId}."
        );

    private readonly IWishlistRepository _wishlistRepository = wishlistRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IEmailService _emailService = emailService;
    private readonly ILogger<ProductBackInStockDomainEventHandler> _logger = logger;

    public async Task Handle(
        ProductBackInStockDomainEvent domainEvent,
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

            var subject = $"Back in Stock: {product.Name} is available again!";
            var body =
                $"Hello {user.FirstName},\n\nExciting news! \"{product.Name}\" on your wishlist is back in stock with {product.StockQuantity} unit(s) available.\n\nVisit Shopizy to place your order before it runs out!";

            try
            {
                await _emailService.SendAsync(user.Email, subject, body, cancellationToken);
            }
            catch (Exception ex)
            {
                s_sendBackInStockFailed(_logger, user.Email, product.Id.Value, ex);
            }
        }
    }
}
