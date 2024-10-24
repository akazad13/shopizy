using Mapster;
using Shopizy.Application.Common.models;
using Shopizy.Application.Payments.Commands.CreatePaymentSession;
using Shopizy.Contracts.Payment;

namespace Shopizy.Api.Common.Mapping;

public class PaymentMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        _ = config
            .NewConfig<
                (Guid UserId, CreatePaymentSessionRequest request),
                CreatePaymentSessionCommand
            >()
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest, src => src.request);

        _ = config.NewConfig<ChargeResource, PaymentResponse>();

        _ = config.NewConfig<CheckoutSession, PaymentSessionResponse>();
    }
}
