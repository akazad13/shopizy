namespace Shopizy.Api.Common.LoggerMessages;

public static partial class LoggerMessages
{
    [LoggerMessage(
        EventId = 1000,
        Level = LogLevel.Error,
        Message = "An error occurred while fetching category."
    )]
    public static partial void CategoryFetchError(this ILogger logger, Exception ex);

    [LoggerMessage(
        EventId = 1001,
        Level = LogLevel.Error,
        Message = "An error occurred while creating category."
    )]
    public static partial void CategoryCreationError(this ILogger logger, Exception ex);

    [LoggerMessage(
        EventId = 1002,
        Level = LogLevel.Error,
        Message = "An error occur while updating category."
    )]
    public static partial void CategoryUpdateError(this ILogger logger, Exception ex);

    [LoggerMessage(
        EventId = 1003,
        Level = LogLevel.Error,
        Message = "An error occurred while deleting category."
    )]
    public static partial void CategoryDeleteError(this ILogger logger, Exception ex);

    [LoggerMessage(
        EventId = 1004,
        Level = LogLevel.Error,
        Message = "An error occurred while fetching cart."
    )]
    public static partial void CartFetchError(this ILogger logger, Exception ex);

    [LoggerMessage(
        EventId = 1005,
        Level = LogLevel.Error,
        Message = "An error occurred while creating cart."
    )]
    public static partial void CartCreationError(this ILogger logger, Exception ex);

    [LoggerMessage(
        EventId = 1006,
        Level = LogLevel.Error,
        Message = "An error occur while updating cart."
    )]
    public static partial void CartUpdateError(this ILogger logger, Exception ex);

    [LoggerMessage(
        EventId = 1007,
        Level = LogLevel.Error,
        Message = "An error occurred while removing item from cart."
    )]
    public static partial void RemoveItemFromCartError(this ILogger logger, Exception ex);

    [LoggerMessage(
        EventId = 1008,
        Level = LogLevel.Error,
        Message = "An error occurred while fetching order."
    )]
    public static partial void OrderFetchError(this ILogger logger, Exception ex);

    [LoggerMessage(
        EventId = 1009,
        Level = LogLevel.Error,
        Message = "An error occurred while creating order."
    )]
    public static partial void OrderCreationError(this ILogger logger, Exception ex);

    [LoggerMessage(
        EventId = 1010,
        Level = LogLevel.Error,
        Message = "An error occur while cancelling cart."
    )]
    public static partial void CancelOrderError(this ILogger logger, Exception ex);

    [LoggerMessage(
        EventId = 1011,
        Level = LogLevel.Error,
        Message = "An error occurred while fetching product."
    )]
    public static partial void ProductFetchError(this ILogger logger, Exception ex);

    [LoggerMessage(
        EventId = 1012,
        Level = LogLevel.Error,
        Message = "An error occurred while creating product."
    )]
    public static partial void ProductCreationError(this ILogger logger, Exception ex);

    [LoggerMessage(
        EventId = 1013,
        Level = LogLevel.Error,
        Message = "An error occur while updating product."
    )]
    public static partial void ProductUpdateError(this ILogger logger, Exception ex);

    [LoggerMessage(
        EventId = 1014,
        Level = LogLevel.Error,
        Message = "An error occurred while deleting product."
    )]
    public static partial void ProductDeleteError(this ILogger logger, Exception ex);

    [LoggerMessage(
        EventId = 1015,
        Level = LogLevel.Error,
        Message = "An error occurred while adding product image."
    )]
    public static partial void ProductImageAdditionError(this ILogger logger, Exception ex);

    [LoggerMessage(
        EventId = 1016,
        Level = LogLevel.Error,
        Message = "An error occurred while deleting product image."
    )]
    public static partial void ProductImageDeleteError(this ILogger logger, Exception ex);

    [LoggerMessage(
        EventId = 1017,
        Level = LogLevel.Error,
        Message = "An error occurred while fetching user."
    )]
    public static partial void UserFetchError(this ILogger logger, Exception ex);

    [LoggerMessage(
        EventId = 1018,
        Level = LogLevel.Error,
        Message = "An error occurred while updating user."
    )]
    public static partial void UserUpdateError(this ILogger logger, Exception ex);

    [LoggerMessage(
        EventId = 1019,
        Level = LogLevel.Error,
        Message = "An error occur while updating user address."
    )]
    public static partial void UserAddressUpdateError(this ILogger logger, Exception ex);

    [LoggerMessage(
        EventId = 1020,
        Level = LogLevel.Error,
        Message = "An error occur while updating user password."
    )]
    public static partial void UserPasswordUpdateError(this ILogger logger, Exception ex);

    [LoggerMessage(
        EventId = 1021,
        Level = LogLevel.Error,
        Message = "An error occur while payment."
    )]
    public static partial void PaymentError(this ILogger logger, Exception ex);

    [LoggerMessage(
        EventId = 1022,
        Level = LogLevel.Error,
        Message = "An error occur while registering user."
    )]
    public static partial void UserRegisterError(this ILogger logger, Exception ex);

    [LoggerMessage(EventId = 1023, Level = LogLevel.Error, Message = "An error occur while login.")]
    public static partial void UserLoginError(this ILogger logger, Exception ex);
}
