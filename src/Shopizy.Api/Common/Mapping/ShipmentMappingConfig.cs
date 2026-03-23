using Mapster;
using Shopizy.Application.Orders.Commands.CreateShipment;
using Shopizy.Application.Orders.Commands.UpdateShipment;
using Shopizy.Contracts.Order;
using Shopizy.Domain.Orders.Entities;

namespace Shopizy.Api.Common.Mapping;

public class ShipmentMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        ArgumentNullException.ThrowIfNull(config);

        config
            .NewConfig<(Guid OrderId, CreateShipmentRequest req), CreateShipmentCommand>()
            .Map(dest => dest.OrderId, src => src.OrderId)
            .Map(dest => dest.Carrier, src => src.req.Carrier)
            .Map(dest => dest.TrackingNumber, src => src.req.TrackingNumber)
            .Map(dest => dest.EstimatedDelivery, src => src.req.EstimatedDelivery);

        config
            .NewConfig<(Guid OrderId, UpdateShipmentRequest req), UpdateShipmentCommand>()
            .Map(dest => dest.OrderId, src => src.OrderId)
            .Map(dest => dest.Carrier, src => src.req.Carrier)
            .Map(dest => dest.TrackingNumber, src => src.req.TrackingNumber)
            .Map(dest => dest.EstimatedDelivery, src => src.req.EstimatedDelivery)
            .Map(dest => dest.Status, src => src.req.Status);

        config
            .NewConfig<Shipment, ShipmentResponse>()
            .Map(dest => dest.ShipmentId, src => src.Id.Value)
            .Map(dest => dest.Carrier, src => src.Carrier)
            .Map(dest => dest.TrackingNumber, src => src.TrackingNumber)
            .Map(dest => dest.EstimatedDelivery, src => src.EstimatedDelivery)
            .Map(dest => dest.Status, src => src.Status.ToString())
            .Map(dest => dest.CreatedOn, src => src.CreatedOn);
    }
}
