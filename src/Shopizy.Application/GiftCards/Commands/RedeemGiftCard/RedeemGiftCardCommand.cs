using ErrorOr;
using Shopizy.Domain.GiftCards;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.GiftCards.Commands.RedeemGiftCard;

public record RedeemGiftCardCommand(UserId UserId, string Code) : ICommand<ErrorOr<GiftCard>>;
