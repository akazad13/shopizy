using Ardalis.GuardClauses;
using Mapster;
using Shopizy.Application.Payments.Commands.CardNotPresentSale;
using Shopizy.Application.Payments.Commands.CashOnDeliverySale;
using Shopizy.Contracts.Payment;

namespace Shopizy.Api.Common.Mapping;

public class PaymentMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        Guard.Against.Null(config);

        config
            .NewConfig<
                (Guid UserId, CardNotPresentSaleRequest request),
                CardNotPresentSaleCommand
            >()
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest.OrderId, src => src.request.OrderId)
            .Map(dest => dest.Amount, src => src.request.Amount)
            .Map(dest => dest.Currency, src => src.request.Currency)
            .Map(dest => dest.PaymentMethod, src => src.request.PaymentMethod)
            .Map(dest => dest.PaymentMethodId, src => src.request.PaymentMethodId)
            .Map(dest => dest.CardName, src => src.request.CardInfo.CardName)
            .Map(dest => dest.CardExpiryMonth, src => src.request.CardInfo.CardExpiryMonth)
            .Map(dest => dest.CardExpiryYear, src => src.request.CardInfo.CardExpiryYear)
            .Map(dest => dest.LastDigits, src => src.request.CardInfo.LastDigits);

        config
            .NewConfig<
                (Guid UserId, CardNotPresentSaleRequest request),
                CashOnDeliverySaleCommand
            >()
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest.OrderId, src => src.request.OrderId)
            .Map(dest => dest.Amount, src => src.request.Amount)
            .Map(dest => dest.Currency, src => src.request.Currency)
            .Map(dest => dest.PaymentMethod, src => src.request.PaymentMethod);
    }
}
