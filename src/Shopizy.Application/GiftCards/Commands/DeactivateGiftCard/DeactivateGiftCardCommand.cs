using ErrorOr;
using Shopizy.Domain.GiftCards;
using Shopizy.Domain.GiftCards.ValueObjects;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.GiftCards.Commands.DeactivateGiftCard;

public record DeactivateGiftCardCommand(GiftCardId GiftCardId) : ICommand<ErrorOr<GiftCard>>;
