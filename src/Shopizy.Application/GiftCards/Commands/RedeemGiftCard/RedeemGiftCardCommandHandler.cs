using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.GiftCards;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.GiftCards.Commands.RedeemGiftCard;

public class RedeemGiftCardCommandHandler(IGiftCardRepository giftCardRepository)
    : ICommandHandler<RedeemGiftCardCommand, ErrorOr<GiftCard>>
{
    private readonly IGiftCardRepository _giftCardRepository = giftCardRepository;

    public async Task<ErrorOr<GiftCard>> Handle(
        RedeemGiftCardCommand request,
        CancellationToken cancellationToken
    )
    {
        var giftCard = await _giftCardRepository.GetByCodeAsync(request.Code);
        if (giftCard is null)
        {
            return (Error)CustomErrors.GiftCard.GiftCardNotFound;
        }

        var redeemResult = giftCard.Redeem(request.UserId);
        if (redeemResult.IsError)
        {
            return (Error)redeemResult.Error.ToError();
        }

        _giftCardRepository.Update(giftCard);

        return giftCard;
    }
}
