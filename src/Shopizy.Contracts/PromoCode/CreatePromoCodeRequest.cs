namespace Shopizy.Contracts.PromoCode;

public record CreatePromoCodeRequest(
    string Code,
    string Description,
    decimal Discount,
    bool IsPercentage,
    bool IsActive
);
