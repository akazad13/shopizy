using Shopizy.Application.Common.models;
using Shopizy.Application.Common.Wrappers;

namespace Shopizy.Application.Common.Interfaces.Services;

public interface IPaymentService
{
    Task<IResult<CustomerResource>> CreateCustomer(
        string email,
        string name,
        CancellationToken cancellationToken
    );
    Task<IResult<CheckoutSession>> CreateCheckoutSession(
        string customerEmail,
        decimal price,
        string successUrl,
        string cancelUrl,
        CancellationToken cancellationToken = default
    );
}
