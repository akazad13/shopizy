namespace Shopizy.Contracts.GiftCard;

/// <summary>
/// Request contract for redeeming a gift card.
/// </summary>
/// <param name="Code">The unique gift card code to redeem.</param>
public record RedeemGiftCardRequest(string Code);
