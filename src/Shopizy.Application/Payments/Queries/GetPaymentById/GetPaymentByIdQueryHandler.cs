using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Payments;
using Shopizy.Domain.Payments.ValueObjects;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Application.Payments.Queries.GetPaymentById;

public class GetPaymentByIdQueryHandler(IPaymentRepository paymentRepository)
    : IQueryHandler<GetPaymentByIdQuery, ErrorOr<Payment>>
{
    private readonly IPaymentRepository _paymentRepository = paymentRepository;

    public async Task<ErrorOr<Payment>> Handle(
        GetPaymentByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var paymentId = PaymentId.Create(request.PaymentId);
        var payment = await _paymentRepository.GetPaymentByIdAsync(paymentId);

        if (payment is null)
        {
            return Error.NotFound("Payment.NotFound", "Payment not found.");
        }

        return payment;
    }
}
