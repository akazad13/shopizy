using ErrorOr;

namespace Shopizy.Domain.Common.CustomErrors;

public static partial class CustomErrors
{
    public static class PromoCode
    {
        public static Error PromoCodeNotFound =>
            Error.NotFound(code: "PromoCode.PromoCodeNotFound", description: "Promo code is not found.");
        public static Error PromoCodeNotCreated =>
            Error.Failure(code: "PromoCode.PromoCodeNotCreated", description: "Failed to create promo code.");
        public static Error PromoCodeInactive =>
            Error.Validation(code: "PromoCode.PromoCodeInactive", description: "This promo code is not active.");
        public static Error DuplicateCode =>
            Error.Conflict(code: "PromoCode.DuplicateCode", description: "A promo code with this code already exists.");
    }
}
