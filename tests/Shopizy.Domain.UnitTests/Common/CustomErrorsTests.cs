using Shopizy.Domain.Common.CustomErrors;
using Shouldly;
using Xunit;

namespace Shopizy.Domain.UnitTests.Common;

public class CustomErrorsTests
{
    [Fact]
    public void CustomErrors_Properties_ShouldReturnExpectedErrorDetails()
    {
        // Category
        CustomErrors.Category.CategoryNotFound.Code.ShouldBe("Category.CategoryNotFound");
        CustomErrors.Category.DuplicateName.Code.ShouldBe("Category.DuplicateName");
        CustomErrors.Category.CategoryNotCreated.Code.ShouldBe("Category.CategoryNotCreated");
        CustomErrors.Category.CategoryNotUpdated.Code.ShouldBe("Category.CategoryNotUpdated");
        CustomErrors.Category.CategoryNotDeleted.Code.ShouldBe("Category.CategoryNotDeleted");

        // LoyaltyAccount
        CustomErrors.LoyaltyAccount.AccountNotFound.Code.ShouldBe("LoyaltyAccount.AccountNotFound");
        CustomErrors.LoyaltyAccount.InsufficientPoints.Code.ShouldBe(
            "LoyaltyAccount.InsufficientPoints"
        );
        CustomErrors.LoyaltyAccount.AccountNotCreated.Code.ShouldBe(
            "LoyaltyAccount.AccountNotCreated"
        );

        // Payment
        CustomErrors.Payment.CustomerNotCreated.Code.ShouldBe("Payment.CustomerNotCreated");
        CustomErrors.Payment.PaymentNotCreated.Code.ShouldBe("Payment.PaymentNotCreated");

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

        // ProductVariant
        CustomErrors.ProductVariant.VariantNotFound.Code.ShouldBe("ProductVariant.VariantNotFound");
        CustomErrors.ProductVariant.VariantNotCreated.Code.ShouldBe(
            "ProductVariant.VariantNotCreated"
        );

        // User
        CustomErrors.User.UserNotCreated.Code.ShouldBe("User.UserNotCreated");
        CustomErrors.User.UserNotUpdated.Code.ShouldBe("User.UserNotUpdated");
        CustomErrors.User.PasswordNotUpdated.Code.ShouldBe("User.PasswordNotUpdated");
        CustomErrors.User.InvalidPhoneFormat.Code.ShouldBe("User.InvalidPhoneFormat");
        CustomErrors.User.InvalidEmailFormat.Code.ShouldBe("User.InvalidEmailFormat");

        // Wishlist
        CustomErrors.Wishlist.WishlistNotFound.Code.ShouldBe("Wishlist.WishlistNotFound");
        CustomErrors.Wishlist.WishlistNotCreated.Code.ShouldBe("Wishlist.WishlistNotCreated");
        CustomErrors.Wishlist.ProductAlreadyInWishlist.Code.ShouldBe(
            "Wishlist.ProductAlreadyInWishlist"
        );
        CustomErrors.Wishlist.ProductNotInWishlist.Code.ShouldBe("Wishlist.ProductNotInWishlist");

        // PromoCode
        CustomErrors.PromoCode.PromoCodeNotFound.Code.ShouldBe("PromoCode.PromoCodeNotFound");
        CustomErrors.PromoCode.PromoCodeNotCreated.Code.ShouldBe("PromoCode.PromoCodeNotCreated");
        CustomErrors.PromoCode.PromoCodeInactive.Code.ShouldBe("PromoCode.PromoCodeInactive");
        CustomErrors.PromoCode.DuplicateCode.Code.ShouldBe("PromoCode.DuplicateCode");

        // ProductReview
        CustomErrors.ProductReview.ReviewNotCreated.Code.ShouldBe("ProductReview.ReviewNotCreated");
        CustomErrors.ProductReview.DuplicateReview.Code.ShouldBe("ProductReview.DuplicateReview");

        // Product
        CustomErrors.Product.DuplicateName.Code.ShouldBe("Product.DuplicateName");
        CustomErrors.Product.ProductNotUpdated.Code.ShouldBe("Product.ProductNotUpdated");
        CustomErrors.Product.ProductNotDeleted.Code.ShouldBe("Product.ProductNotDeleted");
        CustomErrors.Product.ProductImageNotAdded.Code.ShouldBe("Product.ProductImageNotAdded");
        CustomErrors.Product.ProductImageNotFound.Code.ShouldBe("Product.ProductImageNotFound");
        CustomErrors.Product.InsufficientStock.Code.ShouldBe("Product.InsufficientStock");

        // Order
        CustomErrors.Order.OrderNotCreated.Code.ShouldBe("Order.OrderNotCreated");
        CustomErrors.Order.OrderNotDeleted.Code.ShouldBe("Order.OrderNotDeleted");
        CustomErrors.Order.OrderNotCancelled.Code.ShouldBe("Order.OrderNotCancelled");
        CustomErrors.Order.OrderNotUpdated.Code.ShouldBe("Order.OrderNotUpdated");
    }
}
