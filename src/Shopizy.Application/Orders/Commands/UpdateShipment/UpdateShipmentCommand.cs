using ErrorOr;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Orders.Commands.UpdateShipment;

public record UpdateShipmentCommand(
    Guid OrderId,
    string Carrier,
    string TrackingNumber,
    DateTime? EstimatedDelivery,
    int Status
) : ICommand<ErrorOr<Success>>;
