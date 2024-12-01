using Ardalis.GuardClauses;
using Mapster;
using Shopizy.Application.Common.models;
using Shopizy.Application.Payments.Commands.CardNotPresentSale;
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
            .Map(dest => dest, src => src.request);
    }
}
