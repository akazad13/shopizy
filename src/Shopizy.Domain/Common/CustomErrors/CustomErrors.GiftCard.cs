using ErrorOr;

namespace Shopizy.Domain.Common.CustomErrors;

public static partial class CustomErrors
{
    public static class GiftCard
    {
        public static Error GiftCardNotFound =>
            Error.NotFound(code: "GiftCard.GiftCardNotFound", description: "Gift card is not found.");
        public static Error GiftCardAlreadyRedeemed =>
            Error.Conflict(code: "GiftCard.GiftCardAlreadyRedeemed", description: "This gift card has already been redeemed.");
        public static Error GiftCardExpired =>
            Error.Validation(code: "GiftCard.GiftCardExpired", description: "This gift card has expired.");
        public static Error GiftCardInactive =>
            Error.Validation(code: "GiftCard.GiftCardInactive", description: "This gift card is not active.");
        public static Error GiftCardNotCreated =>
            Error.Failure(code: "GiftCard.GiftCardNotCreated", description: "Failed to create gift card.");
        public static Error DuplicateCode =>
            Error.Conflict(code: "GiftCard.DuplicateCode", description: "A gift card with this code already exists.");
    }
}
