using ErrorOr;
using Shopizy.Application.Common.models;

namespace Shopizy.Application.Common.Interfaces.Services;

public interface IPaymentService
{
    Task<ErrorOr<CustomerResource>> CreateCustomer(
        string email,
        string name,
        CancellationToken cancellationToken
    );
    Task<ErrorOr<CheckoutSession>> CreateCheckoutSession(
        string customerEmail,
        decimal price,
        string successUrl,
        string cancelUrl,
        CancellationToken cancellationToken = default
    );
}
