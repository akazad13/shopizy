using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Domain.Common.CustomErrors;

public static partial class CustomErrors
{
    public static class PromoCode
    {
        public static DomainError PromoCodeNotFound =>
            DomainError.NotFound(code: "PromoCode.PromoCodeNotFound", description: "Promo code is not found.");
        public static DomainError PromoCodeNotCreated =>
            DomainError.Failure(code: "PromoCode.PromoCodeNotCreated", description: "Failed to create promo code.");
        public static DomainError PromoCodeInactive =>
            DomainError.Validation(code: "PromoCode.PromoCodeInactive", description: "This promo code is not active.");
        public static DomainError DuplicateCode =>
            DomainError.Conflict(code: "PromoCode.DuplicateCode", description: "A promo code with this code already exists.");
    }
}
