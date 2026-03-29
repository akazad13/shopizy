using ErrorOr;
using Shopizy.Domain.Orders.Entities;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Orders.Queries.GetShipment;

public record GetShipmentQuery(Guid UserId, Guid OrderId) : IQuery<ErrorOr<Shipment>>;
