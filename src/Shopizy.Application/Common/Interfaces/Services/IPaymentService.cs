using ErrorOr;
using Shopizy.Application.Common.models;

namespace Shopizy.Application.Common.Interfaces.Services;

public interface IPaymentService
{
    Task<ErrorOr<CustomerResource>> CreateCustomer(
        string email,
        string name,
        string number,
        string expiryYear,
        string expiryMonth,
        string cvc,
        CancellationToken cancellationToken
    );

    Task<ErrorOr<ChargeResource>> CreateCharge(
        string currency,
        long amount,
        string receiptEmail,
        string customerId,
        string description,
        CancellationToken cancellationToken
    );

    Task<ErrorOr<CheckoutSession>> CreateCheckoutSession(
        string customerId,
        decimal price,
        string successUrl,
        string cancelUrl,
        CancellationToken cancellationToken = default
    );
}
