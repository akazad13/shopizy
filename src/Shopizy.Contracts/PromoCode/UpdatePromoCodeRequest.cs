namespace Shopizy.Contracts.PromoCode;

public record UpdatePromoCodeRequest(
    string Code,
    string Description,
    decimal Discount,
    bool IsPercentage,
    bool IsActive
);
