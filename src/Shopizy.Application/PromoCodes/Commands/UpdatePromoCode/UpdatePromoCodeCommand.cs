using ErrorOr;
using Shopizy.Domain.PromoCodes;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.PromoCodes.Commands.UpdatePromoCode;

public record UpdatePromoCodeCommand(
    Guid PromoCodeId,
    string Code,
    string Description,
    decimal Discount,
    bool IsPercentage,
    bool IsActive
) : ICommand<ErrorOr<PromoCode>>;
