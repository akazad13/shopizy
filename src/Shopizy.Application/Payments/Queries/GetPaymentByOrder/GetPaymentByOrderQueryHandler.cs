using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.Domain.Payments;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Application.Payments.Queries.GetPaymentByOrder;

public class GetPaymentByOrderQueryHandler(IPaymentRepository paymentRepository)
    : IQueryHandler<GetPaymentByOrderQuery, ErrorOr<Payment>>
{
    private readonly IPaymentRepository _paymentRepository = paymentRepository;

    public async Task<ErrorOr<Payment>> Handle(
        GetPaymentByOrderQuery request,
        CancellationToken cancellationToken
    )
    {
        var orderId = OrderId.Create(request.OrderId);
        var payment = await _paymentRepository.GetPaymentByOrderIdAsync(orderId);

        if (payment is null)
        {
            return Error.NotFound("Payment.NotFound", "Payment for the given order not found.");
        }

        return payment;
    }
}
