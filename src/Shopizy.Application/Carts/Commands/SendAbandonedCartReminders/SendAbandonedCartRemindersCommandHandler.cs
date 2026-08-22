using ErrorOr;
using Microsoft.Extensions.Logging;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.SharedKernel.Application.Interfaces.Persistence;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Carts.Commands.SendAbandonedCartReminders;

/// <summary>
/// Handler that queries abandoned shopping carts, notifies users via email, and records reminder timestamps.
/// </summary>
public sealed class SendAbandonedCartRemindersCommandHandler(
    ICartRepository cartRepository,
    IUserRepository userRepository,
    IEmailService emailService,
    IUnitOfWork unitOfWork,
    ILogger<SendAbandonedCartRemindersCommandHandler> logger
) : ICommandHandler<SendAbandonedCartRemindersCommand, ErrorOr<int>>
{
    private readonly ICartRepository _cartRepository = cartRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IEmailService _emailService = emailService;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<SendAbandonedCartRemindersCommandHandler> _logger = logger;

    public async Task<ErrorOr<int>> Handle(
        SendAbandonedCartRemindersCommand request,
        CancellationToken cancellationToken
    )
    {
        var abandonedCarts = await _cartRepository.GetAbandonedCartsAsync(
            request.InactiveBeforeUtc,
            request.MaxBatchSize,
            cancellationToken
        );

        if (abandonedCarts.Count == 0)
        {
            return 0;
        }

        var sentCount = 0;
        var nowUtc = DateTime.UtcNow;

        foreach (var cart in abandonedCarts)
        {
            var user = await _userRepository.GetUserByIdAsync(cart.UserId);
            if (user is not null && !string.IsNullOrWhiteSpace(user.Email))
            {
                var itemCount = cart.CartItems.Sum(i => i.Quantity);
                var subject = "You left items in your cart at Shopizy!";
                var body =
                    $"Hello {user.FirstName},\n\nYou left {itemCount} item(s) in your shopping cart. Don't miss out on your favorite items — complete your checkout today!";

                try
                {
                    await _emailService.SendAsync(user.Email, subject, body, cancellationToken);
                    sentCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Failed to send abandoned cart reminder to {Email} for cart {CartId}.",
                        user.Email,
                        cart.Id.Value
                    );
                }
            }

            cart.RecordAbandonedReminderSent(nowUtc);
            _cartRepository.Update(cart);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return sentCount;
    }
}
