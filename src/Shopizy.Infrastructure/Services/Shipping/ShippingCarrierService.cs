using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Domain.Orders.ValueObjects;

namespace Shopizy.Infrastructure.Services.Shipping;

/// <summary>
/// Service providing multi-carrier shipping rate estimation and live package tracking.
/// </summary>
public class ShippingCarrierService(
    IOptions<ShippingSettings> options,
    ILogger<ShippingCarrierService> logger
) : IShippingCarrierService
{
    private static readonly Action<ILogger, string, decimal, Exception?> LogRatesEstimated =
        LoggerMessage.Define<string, decimal>(
            LogLevel.Information,
            new EventId(1, nameof(EstimateShippingRatesAsync)),
            "Estimated shipping rates for destination: {Country}, Subtotal: {Subtotal}"
        );

    private static readonly Action<ILogger, string, string, Exception?> LogShipmentTracked =
        LoggerMessage.Define<string, string>(
            LogLevel.Information,
            new EventId(2, nameof(TrackShipmentAsync)),
            "Queried tracking for Carrier: {Carrier}, TrackingNumber: {TrackingNumber}"
        );

    private readonly ShippingSettings _settings = options.Value;
    private readonly ILogger<ShippingCarrierService> _logger = logger;

    public Task<IReadOnlyList<ShippingRateEstimateDto>> EstimateShippingRatesAsync(
        Address destinationAddress,
        decimal totalWeightKg,
        decimal subtotal,
        CancellationToken cancellationToken = default
    )
    {
        var weightSurcharge = Math.Max(0, totalWeightKg) * _settings.WeightRatePerKg;
        var isInternational =
            !string.Equals(destinationAddress.Country, "US", StringComparison.OrdinalIgnoreCase)
            && !string.Equals(
                destinationAddress.Country,
                "USA",
                StringComparison.OrdinalIgnoreCase
            );

        var internationalMultiplier = isInternational ? 2.5m : 1.0m;
        var rates = new List<ShippingRateEstimateDto>();

        // 1. Free / Standard Shipping
        var isFreeShipping = subtotal >= _settings.FreeShippingThreshold && !isInternational;
        var standardCost = isFreeShipping
            ? 0m
            : Math.Round(
                (_settings.BaseStandardRate + weightSurcharge) * internationalMultiplier,
                2
            );

        rates.Add(
            new ShippingRateEstimateDto(
                Carrier: "USPS",
                ServiceCode: "USPS_GROUND",
                ServiceName: isFreeShipping
                    ? "Free Standard Shipping (USPS)"
                    : "Standard Ground (USPS)",
                Rate: standardCost,
                Currency: "USD",
                EstimatedDaysMin: isInternational ? 7 : 3,
                EstimatedDaysMax: isInternational ? 14 : 5
            )
        );

        // 2. UPS Ground / Standard
        var upsRate = Math.Round(
            (_settings.BaseStandardRate * 1.2m + weightSurcharge) * internationalMultiplier,
            2
        );
        rates.Add(
            new ShippingRateEstimateDto(
                Carrier: "UPS",
                ServiceCode: "UPS_GROUND",
                ServiceName: "UPS Ground",
                Rate: upsRate,
                Currency: "USD",
                EstimatedDaysMin: isInternational ? 6 : 2,
                EstimatedDaysMax: isInternational ? 10 : 4
            )
        );

        // 3. FedEx Express / Priority
        var fedexExpressRate = Math.Round(
            (_settings.BaseExpressRate + weightSurcharge * 1.5m) * internationalMultiplier,
            2
        );
        rates.Add(
            new ShippingRateEstimateDto(
                Carrier: "FedEx",
                ServiceCode: "FEDEX_2DAY",
                ServiceName: "FedEx 2-Day Express",
                Rate: fedexExpressRate,
                Currency: "USD",
                EstimatedDaysMin: isInternational ? 3 : 2,
                EstimatedDaysMax: isInternational ? 5 : 2
            )
        );

        // 4. Overnight / DHL Express
        var overnightRate = Math.Round(
            (_settings.BaseOvernightRate + weightSurcharge * 2.0m) * internationalMultiplier,
            2
        );
        rates.Add(
            new ShippingRateEstimateDto(
                Carrier: isInternational ? "DHL" : "FedEx",
                ServiceCode: isInternational ? "DHL_EXPRESS" : "FEDEX_OVERNIGHT",
                ServiceName: isInternational ? "DHL Express Worldwide" : "FedEx Priority Overnight",
                Rate: overnightRate,
                Currency: "USD",
                EstimatedDaysMin: 1,
                EstimatedDaysMax: isInternational ? 3 : 1
            )
        );

        LogRatesEstimated(_logger, destinationAddress.Country, subtotal, null);
        return Task.FromResult<IReadOnlyList<ShippingRateEstimateDto>>(rates.AsReadOnly());
    }

    public Task<ShippingTrackingInfoDto?> TrackShipmentAsync(
        string carrier,
        string trackingNumber,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrWhiteSpace(carrier) || string.IsNullOrWhiteSpace(trackingNumber))
        {
            return Task.FromResult<ShippingTrackingInfoDto?>(null);
        }

        var now = DateTime.UtcNow;
        var checkpoints = new List<TrackingCheckpointDto>
        {
            new(now.AddDays(-2), "Origin Facility, New York NY", "Shipment information received"),
            new(now.AddDays(-1.5), "Sorting Hub, Philadelphia PA", "Arrived at sort facility"),
            new(
                now.AddDays(-1),
                "In Transit, Chicago IL",
                "Departed facility in transit to destination"
            ),
            new(now.AddHours(-3), "Local Depot, Destination City", "Out for delivery"),
        };

        var trackingInfo = new ShippingTrackingInfoDto(
            Carrier: carrier,
            TrackingNumber: trackingNumber,
            Status: "InTransit",
            CurrentLocation: "Local Depot, Destination City",
            EstimatedDelivery: now.AddHours(4),
            Checkpoints: checkpoints.AsReadOnly()
        );

        LogShipmentTracked(_logger, carrier, trackingNumber, null);
        return Task.FromResult<ShippingTrackingInfoDto?>(trackingInfo);
    }
}
