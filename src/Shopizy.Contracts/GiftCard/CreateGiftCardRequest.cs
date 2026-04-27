namespace Shopizy.Contracts.GiftCard;

/// <summary>
/// Creates a new gift card with an initial balance.
/// </summary>
/// <param name="Code">Customer-redeemable code (case-insensitive).</param>
/// <param name="InitialBalance">Starting balance. Must be positive.</param>
/// <param name="ExpiresOn">Optional expiration date in UTC.</param>
public record CreateGiftCardRequest(string Code, decimal InitialBalance, DateTime? ExpiresOn);
