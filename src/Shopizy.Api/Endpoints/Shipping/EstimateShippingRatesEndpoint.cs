using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.Extensions;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Application.Shipping.Queries.EstimateShippingRates;
using Shopizy.Contracts.Common;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Api.Endpoints.Shipping;

public record EstimateShippingRatesRequest(
    string Street,
    string City,
    string State,
    string Country,
    string ZipCode,
    decimal TotalWeightKg,
    decimal Subtotal
);

public class EstimateShippingRatesEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost(
                "api/v1.0/shipping/estimate-rates",
                async (
                    [FromBody] EstimateShippingRatesRequest request,
                    [FromServices] IDispatcher mediator,
                    ILogger<EstimateShippingRatesEndpoint> logger
                ) =>
                {
                    var query = new EstimateShippingRatesQuery(
                        request.Street,
                        request.City,
                        request.State,
                        request.Country,
                        request.ZipCode,
                        request.TotalWeightKg,
                        request.Subtotal
                    );

                    return await HandleAsync(
                        mediator,
                        query,
                        rates => Results.Ok(rates),
                        ex => logger.ShippingRateEstimationError(ex)
                    );
                }
            )
            .AllowAnonymous()
            .WithTags("Shipping")
            .WithSummary("Estimate shipping rates")
            .WithDescription(
                "Calculates available shipping carrier rates for the destination address."
            )
            .Produces<IReadOnlyList<ShippingRateEstimateDto>>(StatusCodes.Status200OK)
            .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
}
