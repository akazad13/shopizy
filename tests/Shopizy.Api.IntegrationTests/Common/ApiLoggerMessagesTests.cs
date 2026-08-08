using Microsoft.Extensions.Logging.Abstractions;
using Shopizy.Api.Common.LoggerMessages;
using Shouldly;
using Xunit;

namespace Shopizy.Api.IntegrationTests.Common;

public class ApiLoggerMessagesTests
{
    [Fact]
    public void LoggerMessages_Methods_ShouldExecuteWithoutException()
    {
        var logger = NullLogger.Instance;
        var ex = new Exception("Test Exception");

        Should.NotThrow(() => logger.CategoryFetchError(ex));
        Should.NotThrow(() => logger.CategoryCreationError(ex));
        Should.NotThrow(() => logger.CategoryUpdateError(ex));
        Should.NotThrow(() => logger.CategoryDeleteError(ex));

        Should.NotThrow(() => logger.CartFetchError(ex));
        Should.NotThrow(() => logger.CartCreationError(ex));
        Should.NotThrow(() => logger.CartUpdateError(ex));
        Should.NotThrow(() => logger.RemoveItemFromCartError(ex));

        Should.NotThrow(() => logger.OrderFetchError(ex));
        Should.NotThrow(() => logger.OrderCreationError(ex));
        Should.NotThrow(() => logger.CancelOrderError(ex));

        Should.NotThrow(() => logger.ProductFetchError(ex));
        Should.NotThrow(() => logger.ProductCreationError(ex));
        Should.NotThrow(() => logger.ProductUpdateError(ex));
        Should.NotThrow(() => logger.ProductDeleteError(ex));
        Should.NotThrow(() => logger.ProductImageAdditionError(ex));
        Should.NotThrow(() => logger.ProductImageDeleteError(ex));

        Should.NotThrow(() => logger.UserFetchError(ex));
        Should.NotThrow(() => logger.UserUpdateError(ex));
        Should.NotThrow(() => logger.UserAddressUpdateError(ex));
        Should.NotThrow(() => logger.UserPasswordUpdateError(ex));
        Should.NotThrow(() => logger.UserRegisterError(ex));
        Should.NotThrow(() => logger.UserLoginError(ex));

        Should.NotThrow(() => logger.PaymentError(ex));
        Should.NotThrow(() => logger.UnhandledExceptionError(ex, "/api/test"));

        Should.NotThrow(() => logger.WishlistFetchError(ex));
        Should.NotThrow(() => logger.WishlistCreationError(ex));
        Should.NotThrow(() => logger.WishlistUpdateError(ex));

        Should.NotThrow(() => logger.DashboardMetricsFetchError(ex));
        Should.NotThrow(() => logger.AdminOrdersListFetchError(ex));
        Should.NotThrow(() => logger.AdminOrderDetailFetchError(ex));
        Should.NotThrow(() => logger.OrderStatusUpdateError(ex));

        Should.NotThrow(() => logger.UserRoleUpdateError(ex));
        Should.NotThrow(() => logger.UsersListFetchError(ex));
        Should.NotThrow(() => logger.BrandsFetchError(ex));

        Should.NotThrow(() => logger.ProductReviewFetchError(ex));
        Should.NotThrow(() => logger.ProductReviewCreationError(ex));
        Should.NotThrow(() => logger.ProductReviewDeleteError(ex));

        Should.NotThrow(() => logger.PromoCodeFetchError(ex));
        Should.NotThrow(() => logger.PromoCodeCreationError(ex));
        Should.NotThrow(() => logger.PromoCodeUpdateError(ex));
        Should.NotThrow(() => logger.PromoCodeDeleteError(ex));

        Should.NotThrow(() => logger.BrandFetchError(ex));
        Should.NotThrow(() => logger.BrandCreationError(ex));
        Should.NotThrow(() => logger.BrandUpdateError(ex));
        Should.NotThrow(() => logger.BrandDeleteError(ex));

        Should.NotThrow(() => logger.GiftCardFetchError(ex));
        Should.NotThrow(() => logger.GiftCardCreationError(ex));

        Should.NotThrow(() => logger.LoyaltyAccountFetchError(ex));
        Should.NotThrow(() => logger.LoyaltyPointsEarnError(ex));
        Should.NotThrow(() => logger.LoyaltyPointsRedeemError(ex));

        Should.NotThrow(() => logger.ProductQuestionFetchError(ex));
        Should.NotThrow(() => logger.ProductQuestionCreationError(ex));
        Should.NotThrow(() => logger.ProductQuestionAnswerError(ex));
    }
}
