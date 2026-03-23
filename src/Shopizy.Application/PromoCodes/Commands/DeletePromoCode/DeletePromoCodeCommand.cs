using ErrorOr;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.PromoCodes.Commands.DeletePromoCode;

public record DeletePromoCodeCommand(Guid PromoCodeId) : ICommand<ErrorOr<Deleted>>;
