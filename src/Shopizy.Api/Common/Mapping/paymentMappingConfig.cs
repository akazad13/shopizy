using Mapster;
using Shopizy.Application.Common.models;
using Shopizy.Application.Payments.Commands.CreatePayment;
using Shopizy.Contracts.Payment;

namespace Shopizy.Api.Common.Mapping;

public class PaymentMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        _ = config
            .NewConfig<(Guid UserId, CreatePaymentRequest request), CreatePaymentCommand>()
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest, src => src.request);

        _ = config.NewConfig<CheckoutSession, PaymentResponse>();
    }
}
