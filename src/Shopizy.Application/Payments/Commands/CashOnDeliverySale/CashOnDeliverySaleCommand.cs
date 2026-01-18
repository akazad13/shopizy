using ErrorOr;

namespace Shopizy.Application.Payments.Commands.CashOnDeliverySale;

/// <summary>
/// Represents a command to process a cash-on-delivery payment sale.
/// </summary>
/// <param name="UserId">The user's unique identifier.</param>
/// <param name="OrderId">The order's unique identifier.</param>
/// <param name="Amount">The payment amount.</param>
/// <param name="Currency">The payment currency.</param>
/// <param name="PaymentMethod">The payment method type.</param>
public record CashOnDeliverySaleCommand(
    Guid UserId,
    Guid OrderId,
    decimal Amount,
    string Currency,
    string PaymentMethod
) : MediatR.IRequest<ErrorOr<Success>>;
