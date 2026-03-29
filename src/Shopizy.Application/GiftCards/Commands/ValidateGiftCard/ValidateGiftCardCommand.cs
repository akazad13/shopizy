using ErrorOr;
using Shopizy.Domain.GiftCards;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.GiftCards.Commands.ValidateGiftCard;

public record ValidateGiftCardCommand(string Code) : ICommand<ErrorOr<GiftCard>>;
