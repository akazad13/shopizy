using Shopizy.SharedKernel.Application.Messaging;
using ErrorOr;
using Shopizy.Domain.Orders.Enums;

namespace Shopizy.Application.Orders.Commands.UpdateOrderStatus;

public record UpdateOrderStatusCommand(Guid OrderId, OrderStatus Status) : ICommand<ErrorOr<Success>>;
