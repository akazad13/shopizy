using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.GiftCards;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.GiftCards.Commands.DeactivateGiftCard;

public class DeactivateGiftCardCommandHandler(IGiftCardRepository giftCardRepository)
    : ICommandHandler<DeactivateGiftCardCommand, ErrorOr<GiftCard>>
{
    private readonly IGiftCardRepository _giftCardRepository = giftCardRepository;

    public async Task<ErrorOr<GiftCard>> Handle(
        DeactivateGiftCardCommand request,
        CancellationToken cancellationToken
    )
    {
        var giftCard = await _giftCardRepository.GetByIdAsync(request.GiftCardId);
        if (giftCard is null)
        {
            return (Error)CustomErrors.GiftCard.GiftCardNotFound;
        }

        giftCard.Deactivate();
        _giftCardRepository.Update(giftCard);

        return giftCard;
    }
}
