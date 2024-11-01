namespace Shopizy.Domain.Common.CustomErrors;

public static partial class CustomErrors
{
    public static class Order
    {
        public static string OrderNotFound => "Order is not found.";
        public static string OrderNotCreated => "Failed to create Order.";
        public static string OrderNotDeleted => "Failed to delete Order.";

        public static string OrderNotCancelled => "Failed to cancel Order.";

        public static string OrderNotUpdated => "Failed to update Order.";
    }
}
