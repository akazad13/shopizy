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
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest.OrderId, src => src.request.OrderId)
            .Map(dest => dest.Amount, src => src.request.Amount)
            .Map(dest => dest.Currency, src => src.request.Currency)
            .Map(dest => dest.PaymentMethod, src => src.request.PaymentMethod)
            .Map(dest => dest.PaymentMethodId, src => src.request.PaymentMethodId)
            .Map(dest => dest.CardName, src => src.request.CardInfo != null ? src.request.CardInfo.CardName : string.Empty)
            .Map(dest => dest.CardExpiryMonth, src => src.request.CardInfo != null ? src.request.CardInfo.CardExpiryMonth : 0)
            .Map(dest => dest.CardExpiryYear, src => src.request.CardInfo != null ? src.request.CardInfo.CardExpiryYear : 0)
            .Map(dest => dest.LastDigits, src => src.request.CardInfo != null ? src.request.CardInfo.LastDigits : string.Empty);

        config.NewConfig< (Guid UserId, CardNotPresentSaleRequest request), CashOnDeliverySaleCommand>()
            .Map(dest => dest.UserId, src=> src.UserId)
            .Map(dest => dest, src => src.request);
    }
}
