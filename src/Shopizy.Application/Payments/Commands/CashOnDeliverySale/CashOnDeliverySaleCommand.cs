using ErrorOr;
using Shopizy.Application.Common.Security.Permissions;
using Shopizy.Application.Common.Security.Request;

namespace Shopizy.Application.Payments.Commands.CashOnDeliverySale;

[Authorize(Permissions = Permissions.Order.Create)]
public record CashOnDeliverySaleCommand(
    Guid OrderId,
    decimal Amount,
    string Currency,
    string PaymentMethod
) : IAuthorizeableRequest<ErrorOr<Success>>;
