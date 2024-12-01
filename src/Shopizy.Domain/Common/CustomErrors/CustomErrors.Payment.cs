using ErrorOr;

namespace Shopizy.Domain.Common.CustomErrors;

public static partial class CustomErrors
{
    public static class Payment
    {
        public static Error PaymentNotCreated =>
            Error.Failure(
                code: "payment.PaymentNotCreated",
                description: "Failed to create Payment."
            );
        public static Error CustomerNotCreated =>
            Error.Failure(
                code: "payment.CustomerNotCreated",
                description: "Failed to create customer."
            );
    }
}
