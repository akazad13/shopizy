using ErrorOr;
using Shopizy.Domain.GiftCards;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.GiftCards.Queries.GetGiftCards;

public record GetGiftCardsQuery(int PageNumber, int PageSize) : IQuery<ErrorOr<IReadOnlyList<GiftCard>>>;
