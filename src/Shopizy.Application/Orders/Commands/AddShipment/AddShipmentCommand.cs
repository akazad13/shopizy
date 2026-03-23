using ErrorOr;
using Shopizy.Domain.Orders.Entities;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Orders.Commands.AddShipment;

public record AddShipmentCommand(
    Guid OrderId,
    string Carrier,
    string TrackingNumber,
    DateTime? EstimatedDelivery
) : ICommand<ErrorOr<Shipment>>;
