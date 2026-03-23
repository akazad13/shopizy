using ErrorOr;
using Shopizy.Domain.Orders.Entities;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Orders.Commands.CreateShipment;

public record CreateShipmentCommand(
    Guid OrderId,
    string Carrier,
    string TrackingNumber,
    DateTime? EstimatedDelivery
) : ICommand<ErrorOr<Shipment>>;
