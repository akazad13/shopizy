using ErrorOr;
using Shopizy.Domain.GiftCards;
using Shopizy.Domain.GiftCards.ValueObjects;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.GiftCards.Queries.GetGiftCardById;

public record GetGiftCardByIdQuery(GiftCardId GiftCardId) : IQuery<ErrorOr<GiftCard>>;
