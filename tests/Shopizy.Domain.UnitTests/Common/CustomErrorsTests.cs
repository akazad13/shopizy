using Shopizy.Domain.Common.CustomErrors;
using Shouldly;
using Xunit;

namespace Shopizy.Domain.UnitTests.Common;

public class CustomErrorsTests
{
    [Fact]
    public void CustomErrors_Properties_ShouldReturnExpectedErrorDetails()
    {
        // Authentication
        CustomErrors.Authentication.InvalidCredentials.Code.ShouldBe("Auth.InvalidCred");

        // Brand
        CustomErrors.Brand.DuplicateName.Code.ShouldBe("Brand.DuplicateName");
        CustomErrors.Brand.BrandNotFound.Code.ShouldBe("Brand.BrandNotFound");
        CustomErrors.Brand.BrandNotCreated.Code.ShouldBe("Brand.BrandNotCreated");
        CustomErrors.Brand.BrandNotUpdated.Code.ShouldBe("Brand.BrandNotUpdated");
        CustomErrors.Brand.BrandNotDeleted.Code.ShouldBe("Brand.BrandNotDeleted");

        // Cart
        CustomErrors.Cart.CartNotFound.Code.ShouldBe("Cart.CartNotFound");
        CustomErrors.Cart.CartNotCreated.Code.ShouldBe("Cart.CartNotCreated");
        CustomErrors.Cart.CartNotDeleted.Code.ShouldBe("Cart.CartNotDeleted");
        CustomErrors.Cart.CartProductNotAdded.Code.ShouldBe("Cart.CartProductNotAdded");
        CustomErrors.Cart.CartProductNotFound.Code.ShouldBe("Cart.CartProductNotFound");
        CustomErrors.Cart.CartNotUpdated.Code.ShouldBe("Cart.CartNotUpdated");
        CustomErrors.Cart.CartProductNotRemoved.Code.ShouldBe("Cart.CartProductNotRemoved");
        CustomErrors.Cart.ProductAlreadyExistInCart.Code.ShouldBe("Cart.ProductAlreadyExistInCart");

        // Category
        CustomErrors.Category.CategoryNotFound.Code.ShouldBe("Category.CategoryNotFound");
        CustomErrors.Category.DuplicateName.Code.ShouldBe("Category.DuplicateName");
        CustomErrors.Category.CategoryNotCreated.Code.ShouldBe("Category.CategoryNotCreated");
        CustomErrors.Category.CategoryNotUpdated.Code.ShouldBe("Category.CategoryNotUpdated");
        CustomErrors.Category.CategoryNotDeleted.Code.ShouldBe("Category.CategoryNotDeleted");

        // GiftCard
        CustomErrors.GiftCard.GiftCardNotFound.Code.ShouldBe("GiftCard.GiftCardNotFound");
        CustomErrors.GiftCard.GiftCardAlreadyRedeemed.Code.ShouldBe(
            "GiftCard.GiftCardAlreadyRedeemed"
        );
        CustomErrors.GiftCard.GiftCardExpired.Code.ShouldBe("GiftCard.GiftCardExpired");
        CustomErrors.GiftCard.GiftCardInactive.Code.ShouldBe("GiftCard.GiftCardInactive");
        CustomErrors.GiftCard.GiftCardNotCreated.Code.ShouldBe("GiftCard.GiftCardNotCreated");
        CustomErrors.GiftCard.DuplicateCode.Code.ShouldBe("GiftCard.DuplicateCode");
        CustomErrors.GiftCard.GiftCardInsufficientBalance.Code.ShouldBe(
            "GiftCard.GiftCardInsufficientBalance"
        );

        // LoyaltyAccount
        CustomErrors.LoyaltyAccount.AccountNotFound.Code.ShouldBe("LoyaltyAccount.AccountNotFound");
        CustomErrors.LoyaltyAccount.InsufficientPoints.Code.ShouldBe(
            "LoyaltyAccount.InsufficientPoints"
        );
        CustomErrors.LoyaltyAccount.AccountNotCreated.Code.ShouldBe(
            "LoyaltyAccount.AccountNotCreated"
        );

        // Order
        CustomErrors.Order.OrderNotCreated.Code.ShouldBe("Order.OrderNotCreated");
        CustomErrors.Order.OrderNotDeleted.Code.ShouldBe("Order.OrderNotDeleted");
        CustomErrors.Order.OrderNotCancelled.Code.ShouldBe("Order.OrderNotCancelled");
        CustomErrors.Order.OrderNotUpdated.Code.ShouldBe("Order.OrderNotUpdated");

        // Payment
        CustomErrors.Payment.CustomerNotCreated.Code.ShouldBe("Payment.CustomerNotCreated");
        CustomErrors.Payment.PaymentNotCreated.Code.ShouldBe("Payment.PaymentNotCreated");

        // Product
        CustomErrors.Product.DuplicateName.Code.ShouldBe("Product.DuplicateName");
        CustomErrors.Product.ProductNotFound.Code.ShouldBe("Product.ProductNotFound");
        CustomErrors.Product.ProductNotCreated.Code.ShouldBe("Product.ProductNotCreated");
        CustomErrors.Product.ProductNotUpdated.Code.ShouldBe("Product.ProductNotUpdated");
        CustomErrors.Product.ProductNotDeleted.Code.ShouldBe("Product.ProductNotDeleted");
        CustomErrors.Product.ProductImageNotAdded.Code.ShouldBe("Product.ProductImageNotAdded");
        CustomErrors.Product.ProductImageNotFound.Code.ShouldBe("Product.ProductImageNotFound");
        CustomErrors.Product.InsufficientStock.Code.ShouldBe("Product.InsufficientStock");

        // ProductQuestion
        CustomErrors.ProductQuestion.QuestionNotFound.Code.ShouldBe(
            "ProductQuestion.QuestionNotFound"
        );
        CustomErrors.ProductQuestion.QuestionAlreadyAnswered.Code.ShouldBe(
            "ProductQuestion.QuestionAlreadyAnswered"
        );
        CustomErrors.ProductQuestion.QuestionNotCreated.Code.ShouldBe(
            "ProductQuestion.QuestionNotCreated"
        );

        // ProductReview
        CustomErrors.ProductReview.ReviewNotFound.Code.ShouldBe("ProductReview.ReviewNotFound");
        CustomErrors.ProductReview.ReviewNotCreated.Code.ShouldBe("ProductReview.ReviewNotCreated");
        CustomErrors.ProductReview.DuplicateReview.Code.ShouldBe("ProductReview.DuplicateReview");

        // ProductVariant
        CustomErrors.ProductVariant.VariantNotFound.Code.ShouldBe("ProductVariant.VariantNotFound");
        CustomErrors.ProductVariant.VariantNotCreated.Code.ShouldBe(
            "ProductVariant.VariantNotCreated"
        );

        // PromoCode
        CustomErrors.PromoCode.PromoCodeNotFound.Code.ShouldBe("PromoCode.PromoCodeNotFound");
        CustomErrors.PromoCode.PromoCodeNotCreated.Code.ShouldBe("PromoCode.PromoCodeNotCreated");
        CustomErrors.PromoCode.PromoCodeInactive.Code.ShouldBe("PromoCode.PromoCodeInactive");
        CustomErrors.PromoCode.DuplicateCode.Code.ShouldBe("PromoCode.DuplicateCode");

        // ReturnRequest
        CustomErrors.ReturnRequest.ReturnNotFound.Code.ShouldBe("ReturnRequest.ReturnNotFound");
        CustomErrors.ReturnRequest.ReturnNotPending.Code.ShouldBe("ReturnRequest.ReturnNotPending");
        CustomErrors.ReturnRequest.ReturnAlreadyProcessed.Code.ShouldBe(
            "ReturnRequest.ReturnAlreadyProcessed"
        );

        // Shipment
        CustomErrors.Shipment.ShipmentNotFound.Code.ShouldBe("Shipment.ShipmentNotFound");
        CustomErrors.Shipment.TrackingNotFound.Code.ShouldBe("Shipment.TrackingNotFound");

        // User
        CustomErrors.User.UserNotCreated.Code.ShouldBe("User.UserNotCreated");
        CustomErrors.User.UserNotUpdated.Code.ShouldBe("User.UserNotUpdated");
        CustomErrors.User.PasswordNotUpdated.Code.ShouldBe("User.PasswordNotUpdated");
        CustomErrors.User.PasswordNotCorrect.Code.ShouldBe("User.PasswordNotCorrect");
        CustomErrors.User.PasswordSameAsOld.Code.ShouldBe("User.PasswordSameAsOld");
        CustomErrors.User.InvalidName.Code.ShouldBe("User.InvalidName");
        CustomErrors.User.InvalidPhoneFormat.Code.ShouldBe("User.InvalidPhoneFormat");
        CustomErrors.User.InvalidEmailFormat.Code.ShouldBe("User.InvalidEmailFormat");
        CustomErrors.User.DuplicateEmail.Code.ShouldBe("User.DuplicateEmail");
        CustomErrors.User.UserNotFound.Code.ShouldBe("User.UserNotFound");
        CustomErrors.User.UserNotFoundWhileLogin.Code.ShouldBe("User.UserNotFound");

        // UserAddress
        CustomErrors.UserAddress.AddressNotFound.Code.ShouldBe("UserAddress.AddressNotFound");

        // Wishlist
        CustomErrors.Wishlist.WishlistNotFound.Code.ShouldBe("Wishlist.WishlistNotFound");
        CustomErrors.Wishlist.WishlistNotCreated.Code.ShouldBe("Wishlist.WishlistNotCreated");
        CustomErrors.Wishlist.ProductAlreadyInWishlist.Code.ShouldBe(
            "Wishlist.ProductAlreadyInWishlist"
        );
        CustomErrors.Wishlist.ProductNotInWishlist.Code.ShouldBe("Wishlist.ProductNotInWishlist");
    }
}
