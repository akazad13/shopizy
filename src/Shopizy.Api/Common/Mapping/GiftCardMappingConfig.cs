using Mapster;
using Shopizy.Application.GiftCards.Commands.CreateGiftCard;
using Shopizy.Contracts.GiftCard;
using Shopizy.Domain.GiftCards;

namespace Shopizy.Api.Common.Mapping;

public class GiftCardMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        ArgumentNullException.ThrowIfNull(config);

        config
            .NewConfig<CreateGiftCardRequest, CreateGiftCardCommand>()
            .Map(dest => dest.Code, src => src.Code)
            .Map(dest => dest.InitialBalance, src => src.InitialBalance)
            .Map(dest => dest.ExpiresOn, src => src.ExpiresOn);

        config
            .NewConfig<GiftCard, GiftCardResponse>()
            .Map(dest => dest.GiftCardId, src => src.Id.Value)
            .Map(dest => dest.Code, src => src.Code)
            .Map(dest => dest.InitialBalance, src => src.InitialBalance)
            .Map(dest => dest.RemainingBalance, src => src.RemainingBalance)
            .Map(dest => dest.IsActive, src => src.IsActive)
            .Map(dest => dest.ExpiresOn, src => src.ExpiresOn)
            .Map(dest => dest.CreatedOn, src => src.CreatedOn);
    }
}
