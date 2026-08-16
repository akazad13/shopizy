using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Payments;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Payments.Queries.GetPaymentHistory;

public class GetPaymentHistoryQueryHandler(IPaymentRepository paymentRepository)
    : IQueryHandler<GetPaymentHistoryQuery, ErrorOr<IReadOnlyList<Payment>>>
{
    private readonly IPaymentRepository _paymentRepository = paymentRepository;

    public async Task<ErrorOr<IReadOnlyList<Payment>>> Handle(
        GetPaymentHistoryQuery request,
        CancellationToken cancellationToken
    )
    {
        var userId = UserId.Create(request.UserId);
        var payments = await _paymentRepository.GetPaymentsByUserIdAsync(userId);

        return payments.ToList().AsReadOnly();
    }
}
