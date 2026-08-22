using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Domain.Common.CustomErrors;

public static partial class CustomErrors
{
    public static class Shipment
    {
        public static DomainError ShipmentNotFound =>
            DomainError.NotFound(
                code: "Shipment.ShipmentNotFound",
                description: "Shipment is not found for this order."
            );

        public static DomainError TrackingNotFound =>
            DomainError.NotFound(
                code: "Shipment.TrackingNotFound",
                description: "Tracking information is not found."
            );
    }
}
