using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Domain.Common.CustomErrors;

public static partial class CustomErrors
{
    public static class GiftCard
    {
        public static DomainError GiftCardNotFound =>
            DomainError.NotFound(code: "GiftCard.GiftCardNotFound", description: "Gift card is not found.");
        public static DomainError GiftCardAlreadyRedeemed =>
            DomainError.Conflict(code: "GiftCard.GiftCardAlreadyRedeemed", description: "This gift card has already been redeemed.");
        public static DomainError GiftCardExpired =>
            DomainError.Validation(code: "GiftCard.GiftCardExpired", description: "This gift card has expired.");
        public static DomainError GiftCardInactive =>
            DomainError.Validation(code: "GiftCard.GiftCardInactive", description: "This gift card is not active.");
        public static DomainError GiftCardNotCreated =>
            DomainError.Failure(code: "GiftCard.GiftCardNotCreated", description: "Failed to create gift card.");
        public static DomainError DuplicateCode =>
            DomainError.Conflict(code: "GiftCard.DuplicateCode", description: "A gift card with this code already exists.");
    }
}
