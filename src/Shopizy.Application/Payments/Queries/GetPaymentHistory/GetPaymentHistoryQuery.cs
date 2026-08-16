using ErrorOr;
using Shopizy.Domain.Payments;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Payments.Queries.GetPaymentHistory;

public record GetPaymentHistoryQuery(Guid UserId) : IQuery<ErrorOr<IReadOnlyList<Payment>>>;
