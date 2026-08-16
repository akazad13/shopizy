using ErrorOr;
using Shopizy.Domain.Payments;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Payments.Queries.GetPaymentByOrder;

public record GetPaymentByOrderQuery(Guid OrderId) : IQuery<ErrorOr<Payment>>;
