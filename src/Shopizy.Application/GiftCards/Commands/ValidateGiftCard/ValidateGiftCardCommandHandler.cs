using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.GiftCards;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.GiftCards.Commands.ValidateGiftCard;

public class ValidateGiftCardCommandHandler(IGiftCardRepository giftCardRepository)
    : ICommandHandler<ValidateGiftCardCommand, ErrorOr<GiftCard>>
{
    private readonly IGiftCardRepository _giftCardRepository = giftCardRepository;

    public async Task<ErrorOr<GiftCard>> Handle(
        ValidateGiftCardCommand request,
        CancellationToken cancellationToken
    )
    {
        var giftCard = await _giftCardRepository.GetByCodeAsync(request.Code);

        if (giftCard is null)
        {
            return CustomErrors.GiftCard.GiftCardNotFound;
        }

        if (!giftCard.IsActive)
        {
            return CustomErrors.GiftCard.GiftCardInactive;
        }

        if (giftCard.ExpiresOn.HasValue && giftCard.ExpiresOn.Value < DateTime.UtcNow)
        {
            return CustomErrors.GiftCard.GiftCardExpired;
        }

        return giftCard;
    }
}
