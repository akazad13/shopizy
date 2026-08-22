using ErrorOr;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Shipping.Queries.EstimateShippingRates;

public record EstimateShippingRatesQuery(
    string Street,
    string City,
    string State,
    string Country,
    string ZipCode,
    decimal TotalWeightKg,
    decimal Subtotal
) : IQuery<ErrorOr<IReadOnlyList<ShippingRateEstimateDto>>>;
