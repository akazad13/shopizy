using ErrorOr;
using Shopizy.Domain.Orders.Enums;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Orders.Commands.UpdateOrderStatus;

public record UpdateOrderStatusCommand(Guid UserId, Guid OrderId, OrderStatus Status)
    : ICommand<ErrorOr<Success>>;
