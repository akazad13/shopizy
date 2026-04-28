using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Domain.Common.CustomErrors;

public static partial class CustomErrors
{
    public static class Order
    {
        public static DomainError OrderNotFound =>
            DomainError.NotFound(code: "Order.OrderNotFound", description: "Order is not found.");
        public static DomainError OrderNotCreated =>
            DomainError.Failure(
                code: "Order.OrderNotCreated",
                description: "Failed to create Order."
            );
        public static DomainError OrderNotDeleted =>
            DomainError.Failure(
                code: "Order.OrderNotDeleted",
                description: "Failed to delete Order."
            );

        public static DomainError OrderNotCancelled =>
            DomainError.Failure(
                code: "Order.OrderNotCancelled",
                description: "Failed to cancel Order."
            );

        public static DomainError OrderNotUpdated =>
            DomainError.Failure(
                code: "Order.OrderNotUpdated",
                description: "Failed to update Order."
            );
    }
}
