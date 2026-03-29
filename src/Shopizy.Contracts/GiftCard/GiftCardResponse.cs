namespace Shopizy.Contracts.GiftCard;

public record GiftCardResponse(
    Guid GiftCardId,
    string Code,
    decimal InitialBalance,
    decimal RemainingBalance,
    bool IsActive,
    DateTime? ExpiresOn,
    DateTime CreatedOn
);
