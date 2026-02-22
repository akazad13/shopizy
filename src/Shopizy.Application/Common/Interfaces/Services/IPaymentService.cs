using ErrorOr;
using Shopizy.SharedKernel.Application.Models;

namespace Shopizy.Application.Common.Interfaces.Services;

public interface IPaymentService
{
    Task<ErrorOr<CustomerResource>> CreateCustomer(
        string email,
        string name,
        CancellationToken cancellationToken
    );
    Task<ErrorOr<CreateSaleResponse>> CreateSaleAsync(CreateSaleRequest request);
}
