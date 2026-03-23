using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.GiftCards;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.GiftCards.Commands.CreateGiftCard;

public class CreateGiftCardCommandHandler(IGiftCardRepository giftCardRepository)
    : ICommandHandler<CreateGiftCardCommand, ErrorOr<GiftCard>>
{
    private readonly IGiftCardRepository _giftCardRepository = giftCardRepository;

    public async Task<ErrorOr<GiftCard>> Handle(
        CreateGiftCardCommand request,
        CancellationToken cancellationToken
    )
    {
        var existing = await _giftCardRepository.GetByCodeAsync(request.Code);
        if (existing is not null)
        {
            return CustomErrors.GiftCard.DuplicateCode;
        }

        var giftCard = GiftCard.Create(request.Code, request.InitialBalance, request.ExpiresOn);

        await _giftCardRepository.AddAsync(giftCard);

        return giftCard;
    }
}
