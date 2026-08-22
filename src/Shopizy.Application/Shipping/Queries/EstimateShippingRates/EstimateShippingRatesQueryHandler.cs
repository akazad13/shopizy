using ErrorOr;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Shipping.Queries.EstimateShippingRates;

public class EstimateShippingRatesQueryHandler(IShippingCarrierService shippingCarrierService)
    : IQueryHandler<EstimateShippingRatesQuery, ErrorOr<IReadOnlyList<ShippingRateEstimateDto>>>
{
    private readonly IShippingCarrierService _shippingCarrierService = shippingCarrierService;

    public async Task<ErrorOr<IReadOnlyList<ShippingRateEstimateDto>>> Handle(
        EstimateShippingRatesQuery request,
        CancellationToken cancellationToken
    )
    {
        var address = Address.CreateNew(
            request.Street,
            request.City,
            request.State,
            request.Country,
            request.ZipCode
        );

        var rates = await _shippingCarrierService.EstimateShippingRatesAsync(
            address,
            request.TotalWeightKg,
            request.Subtotal,
            cancellationToken
        );

        return rates.ToList().AsReadOnly();
    }
}
