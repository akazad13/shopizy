using ErrorOr;
using Shopizy.Domain.PromoCodes;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.PromoCodes.Commands.CreatePromoCode;

public record CreatePromoCodeCommand(
    string Code,
    string Description,
    decimal Discount,
    bool IsPercentage,
    bool IsActive
) : ICommand<ErrorOr<PromoCode>>;
