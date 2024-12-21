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

        config.NewConfig<CardNotPresentSaleRequest, CardNotPresentSaleCommand>();

        config.NewConfig<CardNotPresentSaleRequest, CashOnDeliverySaleCommand>();
    }
}
