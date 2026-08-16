using ErrorOr;
using Shopizy.Domain.Payments;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Payments.Queries.GetPaymentById;

public record GetPaymentByIdQuery(Guid PaymentId) : IQuery<ErrorOr<Payment>>;
