using ErrorOr;
using Shopizy.Application.Common.Security.Permissions;
using Shopizy.Application.Common.Security.Request;

namespace Shopizy.Application.Orders.Commands.CancelOrder;

/// <summary>
/// Represents a command to cancel an existing order.
/// </summary>
/// <param name="UserId">The user's unique identifier.</param>
/// <param name="OrderId">The order's unique identifier to cancel.</param>
/// <param name="Reason">The reason for cancellation.</param>
[Authorize(Permissions = Permissions.Order.Modify)]
public record CancelOrderCommand(Guid UserId, Guid OrderId, string Reason)
    : IAuthorizeableRequest<ErrorOr<Success>>;
