using ErrorOr;

namespace Shopizy.Domain.Common.CustomErrors;

public static partial class CustomErrors
{
    public static class LoyaltyAccount
    {
        public static Error AccountNotFound =>
            Error.NotFound(code: "LoyaltyAccount.AccountNotFound", description: "Loyalty account is not found.");
        public static Error InsufficientPoints =>
            Error.Validation(code: "LoyaltyAccount.InsufficientPoints", description: "Insufficient loyalty points for this redemption.");
        public static Error AccountNotCreated =>
            Error.Failure(code: "LoyaltyAccount.AccountNotCreated", description: "Failed to create loyalty account.");
    }
}
