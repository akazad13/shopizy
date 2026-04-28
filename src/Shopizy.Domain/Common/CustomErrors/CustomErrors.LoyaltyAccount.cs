using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Domain.Common.CustomErrors;

public static partial class CustomErrors
{
    public static class LoyaltyAccount
    {
        public static DomainError AccountNotFound =>
            DomainError.NotFound(
                code: "LoyaltyAccount.AccountNotFound",
                description: "Loyalty account is not found."
            );
        public static DomainError InsufficientPoints =>
            DomainError.Validation(
                code: "LoyaltyAccount.InsufficientPoints",
                description: "Insufficient loyalty points for this redemption."
            );
        public static DomainError AccountNotCreated =>
            DomainError.Failure(
                code: "LoyaltyAccount.AccountNotCreated",
                description: "Failed to create loyalty account."
            );
    }
}
