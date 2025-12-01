using Ardalis.GuardClauses;
using Mapster;
using Shopizy.Application.Payments.Commands.CardNotPresentSale;
using Shopizy.Application.Payments.Commands.CashOnDeliverySale;
using Shopizy.Contracts.Payment;

namespace Shopizy.Api.Common.Mapping;

/// <summary>
/// Configures mapping for payment-related models.
/// </summary>
public class PaymentMappingConfig : IRegister
{
    /// <summary>
    /// Registers the mapping configurations.
    /// </summary>
    /// <param name="config">The type adapter configuration.</param>
    public void Register(TypeAdapterConfig config)
    {
        Guard.Against.Null(config);

        config.NewConfig<(Guid UserId, CardNotPresentSaleRequest request), CardNotPresentSaleCommand>()
            .Map(dest => dest.UserId, src=> src.UserId)
            .Map(dest => dest, src=> src.request);

        config.NewConfig< (Guid UserId, CardNotPresentSaleRequest request), CashOnDeliverySaleCommand>()
            .Map(dest => dest.UserId, src=> src.UserId)
            .Map(dest => dest, src => src.request);
    }
}
