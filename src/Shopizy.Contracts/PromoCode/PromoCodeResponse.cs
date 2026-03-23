namespace Shopizy.Contracts.PromoCode;

public record PromoCodeResponse(
    Guid PromoCodeId,
    string Code,
    string Description,
    decimal Discount,
    bool IsPercentage,
    bool IsActive,
    int NumOfTimeUsed,
    DateTime CreatedOn
);
