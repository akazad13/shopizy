using Microsoft.AspNetCore.Http;
using Moq;
using Shopizy.Contracts.Admin;
using Shopizy.Contracts.AuditLog;
using Shopizy.Contracts.Authentication;
using Shopizy.Contracts.Cart;
using Shopizy.Contracts.Category;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.GiftCard;
using Shopizy.Contracts.LoyaltyAccount;
using Shopizy.Contracts.Order;
using Shopizy.Contracts.Payment;
using Shopizy.Contracts.Product;
using Shopizy.Contracts.ProductQuestion;
using Shopizy.Contracts.ProductReview;
using Shopizy.Contracts.PromoCode;
using Shopizy.Contracts.User;
using Shopizy.Contracts.Wishlist;
using Shouldly;
using Xunit;

namespace Shopizy.Contracts.UnitTests;

public class ContractDTOsTests
{
    [Fact]
    public void Admin_Contracts_ShouldInitializeProperties()
    {
        var stock = new StockAlertResponse(Guid.NewGuid(), "Product A", 2);
        stock.ShouldNotBeNull();

        var customer = new TopCustomerResponse(Guid.NewGuid(), "Alice", "alice@example.com", 500m);
        customer.ShouldNotBeNull();

        var topProd = new TopProductResponse("Product A", 10, 1000m);
        topProd.ShouldNotBeNull();
    }

    [Fact]
    public void Authentication_Contracts_ShouldInitializeProperties()
    {
        var forgotReq = new ForgotPasswordRequest("user@example.com");
        forgotReq.ShouldNotBeNull();

        var forgotRes = new ForgotPasswordResponse("Reset link sent");
        forgotRes.ShouldNotBeNull();

        var refresh = new RefreshTokenRequest("token123");
        refresh.ShouldNotBeNull();

        var reset = new ResetPasswordRequest("token123", "NewPass123!");
        reset.ShouldNotBeNull();

        var twoFactorRes = new TwoFactorSetupResponse("secretkey", "qrurl");
        twoFactorRes.ShouldNotBeNull();

        var verify2FA = new VerifyTwoFactorRequest("123456");
        verify2FA.ShouldNotBeNull();
    }

    [Fact]
    public void Cart_Contracts_ShouldInitializeProperties()
    {
        var req = new CreateCartWithFirstProductRequest(Guid.NewGuid(), "Red", "M", 2);
        req.ShouldNotBeNull();
    }

    [Fact]
    public void Payment_Contracts_ShouldInitializeProperties()
    {
        var card = new CardInfo("1234", 12, 2028, "Visa");
        card.ShouldNotBeNull();

        var res = new PaymentResponse("ch_123", "succeeded", 1000, "usd", "pm_123", "cus_123");
        res.ShouldNotBeNull();
    }

    [Fact]
    public void Product_Contracts_ShouldInitializeProperties()
    {
        var mockFile = new Mock<IFormFile>();
        var imgReq = new AddProductImageRequest(mockFile.Object);
        imgReq.ShouldNotBeNull();

        var addVarReq = new AddVariantRequest("Variant 1", "SKU1", 50m, "usd", 10);
        addVarReq.ShouldNotBeNull();

        var detailRevRes = new ProductDetailReviewResponse(
            Guid.NewGuid(),
            "John",
            "Great",
            "Awesome product",
            5m,
            DateTime.UtcNow
        );
        detailRevRes.ShouldNotBeNull();

        var imgRes = new ProductImageResponse(
            Guid.NewGuid(),
            "https://img.com/1.jpg",
            1,
            "main image"
        );
        imgRes.ShouldNotBeNull();

        var varRes = new ProductVariantResponse(
            Guid.NewGuid(),
            "Var 1",
            "SKU1",
            50m,
            "usd",
            10,
            true
        );
        varRes.ShouldNotBeNull();

        var updVarReq = new UpdateVariantRequest("Var 1", "SKU1", 55m, "usd", 15, true);
        updVarReq.ShouldNotBeNull();
    }

    [Fact]
    public void ProductQuestion_Contracts_ShouldInitializeProperties()
    {
        var ansReq = new AnswerQuestionRequest("This is the answer");
        ansReq.ShouldNotBeNull();

        var askReq = new AskQuestionRequest("What is the warranty?");
        askReq.ShouldNotBeNull();

        var qRes = new ProductQuestionResponse(
            Guid.NewGuid(),
            "Warranty?",
            true,
            "1 Year",
            DateTime.UtcNow
        );
        qRes.ShouldNotBeNull();
    }

    [Fact]
    public void ProductReview_Contracts_ShouldInitializeProperties()
    {
        var revRes = new ProductReviewResponse(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Alice",
            5m,
            "Great product!",
            DateTime.UtcNow
        );
        revRes.ShouldNotBeNull();
    }

    [Fact]
    public void PromoCode_Contracts_ShouldInitializeProperties()
    {
        var createReq = new CreatePromoCodeRequest("PROMO10", "10% Off", 10m, true, true);
        createReq.ShouldNotBeNull();

        var pRes = new PromoCodeResponse(
            Guid.NewGuid(),
            "PROMO10",
            "10% Off",
            10m,
            true,
            true,
            5,
            DateTime.UtcNow
        );
        pRes.ShouldNotBeNull();

        var updReq = new UpdatePromoCodeRequest("PROMO20", "20% Off", 20m, true, false);
        updReq.ShouldNotBeNull();
    }

    [Fact]
    public void User_Contracts_ShouldInitializeProperties()
    {
        var addAddrReq = new AddAddressRequest(
            "Main St",
            "City",
            "State",
            "Country",
            "12345",
            true
        );
        addAddrReq.ShouldNotBeNull();

        var addUserAddrReq = new AddUserAddressRequest(
            "Main St",
            "City",
            "State",
            "Country",
            "12345",
            true
        );
        addUserAddrReq.ShouldNotBeNull();

        var updUserAddrReq = new UpdateUserAddressRequest(
            "Main St",
            "City",
            "State",
            "Country",
            "12345"
        );
        updUserAddrReq.ShouldNotBeNull();

        var addrRes = new UserAddressResponse(
            Guid.NewGuid(),
            "Main St",
            "City",
            "State",
            "Country",
            "12345",
            true,
            DateTime.UtcNow
        );
        addrRes.ShouldNotBeNull();
    }

    [Fact]
    public void Wishlist_Contracts_ShouldInitializeProperties()
    {
        var updSettingsReq = new UpdateWishlistSettingsRequest("Favorites", true);
        updSettingsReq.ShouldNotBeNull();
    }
}
