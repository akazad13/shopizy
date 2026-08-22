using Shopizy.Domain.Orders.ValueObjects;

namespace Shopizy.Application.Common.Interfaces.Services;

/// <summary>
/// DTO representing an estimated shipping rate from a carrier service.
/// </summary>
public record ShippingRateEstimateDto(
    string Carrier,
    string ServiceCode,
    string ServiceName,
    decimal Rate,
    string Currency,
    int EstimatedDaysMin,
    int EstimatedDaysMax
);

/// <summary>
/// DTO representing live tracking information for a package.
/// </summary>
public record ShippingTrackingInfoDto(
    string Carrier,
    string TrackingNumber,
    string Status,
    string? CurrentLocation,
    DateTime? EstimatedDelivery,
    IReadOnlyList<TrackingCheckpointDto> Checkpoints
);

/// <summary>
/// DTO representing a parcel tracking scan checkpoint.
/// </summary>
public record TrackingCheckpointDto(DateTime TimestampUtc, string Location, string Description);

/// <summary>
/// Service interface for querying live shipping carrier rates and parcel tracking.
/// </summary>
public interface IShippingCarrierService
{
    /// <summary>
    /// Estimates available shipping rates and delivery timeframes for the destination address.
    /// </summary>
    Task<IReadOnlyList<ShippingRateEstimateDto>> EstimateShippingRatesAsync(
        Address destinationAddress,
        decimal totalWeightKg,
        decimal subtotal,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves live tracking scan checkpoints and current shipment status.
    /// </summary>
    Task<ShippingTrackingInfoDto?> TrackShipmentAsync(
        string carrier,
        string trackingNumber,
        CancellationToken cancellationToken = default
    );
}
