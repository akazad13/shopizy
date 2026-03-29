using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Domain.Common.CustomErrors;

public static partial class CustomErrors
{
    public static class Payment
    {
        public static DomainError PaymentNotCreated =>
            DomainError.Failure(
                code: "Payment.PaymentNotCreated",
                description: "Failed to create Payment."
            );
        public static DomainError CustomerNotCreated =>
            DomainError.Failure(
                code: "Payment.CustomerNotCreated",
                description: "Failed to create customer."
            );
    }
}
