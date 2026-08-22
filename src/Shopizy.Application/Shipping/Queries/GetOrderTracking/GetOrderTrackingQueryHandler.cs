using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Shipping.Queries.GetOrderTracking;

public class GetOrderTrackingQueryHandler(
    IOrderRepository orderRepository,
    IShippingCarrierService shippingCarrierService
) : IQueryHandler<GetOrderTrackingQuery, ErrorOr<ShippingTrackingInfoDto>>
{
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IShippingCarrierService _shippingCarrierService = shippingCarrierService;

    public async Task<ErrorOr<ShippingTrackingInfoDto>> Handle(
        GetOrderTrackingQuery request,
        CancellationToken cancellationToken
    )
    {
        var order = await _orderRepository.GetOrderByIdAsync(OrderId.Create(request.OrderId));
        if (order is null)
        {
            return (Error)CustomErrors.Order.OrderNotFound;
        }

        if (order.Shipment is null)
        {
            return (Error)CustomErrors.Shipment.ShipmentNotFound;
        }

        var trackingInfo = await _shippingCarrierService.TrackShipmentAsync(
            order.Shipment.Carrier,
            order.Shipment.TrackingNumber,
            cancellationToken
        );

        if (trackingInfo is null)
        {
            return (Error)CustomErrors.Shipment.TrackingNotFound;
        }

        return trackingInfo;
    }
}
