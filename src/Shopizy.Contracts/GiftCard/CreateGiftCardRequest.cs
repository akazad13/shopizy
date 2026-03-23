namespace Shopizy.Contracts.GiftCard;

public record CreateGiftCardRequest(string Code, decimal InitialBalance, DateTime? ExpiresOn);
