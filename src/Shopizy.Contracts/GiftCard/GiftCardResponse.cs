namespace Shopizy.Contracts.GiftCard;

/// <summary>
/// Gift card record returned by the API.
/// </summary>
/// <param name="GiftCardId">Identifier of the gift card.</param>
/// <param name="Code">Customer-redeemable code.</param>
/// <param name="InitialBalance">Starting balance when the card was created.</param>
/// <param name="RemainingBalance">Current redeemable balance.</param>
/// <param name="IsActive">False once expired or fully redeemed.</param>
/// <param name="ExpiresOn">Optional expiration date in UTC.</param>
/// <param name="CreatedOn">UTC timestamp when the card was created.</param>
public record GiftCardResponse(
    Guid GiftCardId,
    string Code,
    decimal InitialBalance,
    decimal RemainingBalance,
    bool IsActive,
    DateTime? ExpiresOn,
    DateTime CreatedOn
);
