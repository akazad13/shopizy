using ErrorOr;

namespace Shopizy.Domain.Common.CustomErrors;

public static partial class CustomErrors
{
    public static class Payment
    {
        public static Error PaymentNotCreated =>
            Error.Failure(
                code: "Payment.PaymentNotCreated",
                description: "Failed to create Payment."
            );
        public static Error CustomerNotCreated =>
            Error.Failure(
                code: "Payment.CustomerNotCreated",
                description: "Failed to create customer."
            );
    }
}
