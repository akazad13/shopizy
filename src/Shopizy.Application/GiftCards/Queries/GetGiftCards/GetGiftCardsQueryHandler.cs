using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.GiftCards;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.GiftCards.Queries.GetGiftCards;

public class GetGiftCardsQueryHandler(IGiftCardRepository giftCardRepository)
    : IQueryHandler<GetGiftCardsQuery, ErrorOr<IReadOnlyList<GiftCard>>>
{
    private readonly IGiftCardRepository _giftCardRepository = giftCardRepository;

    public async Task<ErrorOr<IReadOnlyList<GiftCard>>> Handle(
        GetGiftCardsQuery request,
        CancellationToken cancellationToken
    )
    {
        var giftCards = await _giftCardRepository.GetAllAsync(request.PageNumber, request.PageSize);
        return ErrorOrFactory.From(giftCards);
    }
}
