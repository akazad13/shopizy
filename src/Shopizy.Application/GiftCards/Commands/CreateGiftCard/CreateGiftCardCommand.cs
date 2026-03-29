using ErrorOr;
using Shopizy.Domain.GiftCards;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.GiftCards.Commands.CreateGiftCard;

public record CreateGiftCardCommand(string Code, decimal InitialBalance, DateTime? ExpiresOn) : ICommand<ErrorOr<GiftCard>>;
