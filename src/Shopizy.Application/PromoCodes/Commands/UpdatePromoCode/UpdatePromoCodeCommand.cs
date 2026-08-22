using ErrorOr;
using Shopizy.Domain.PromoCodes;
using Shopizy.Domain.PromoCodes.Enums;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.PromoCodes.Commands.UpdatePromoCode;

public record UpdatePromoCodeCommand(
    Guid PromoCodeId,
    string Code,
    string Description,
    decimal Discount,
    bool IsPercentage,
    bool IsActive,
    PromoType PromoType = PromoType.Standard,
    decimal? MinimumOrderAmount = null,
    decimal? MaxDiscountAmount = null,
    Guid? TargetCategoryId = null,
    int? BuyQuantity = null,
    int? GetQuantity = null,
    decimal? GetDiscountPercentage = null,
    int? UsageLimit = null,
    DateTime? StartDate = null,
    DateTime? EndDate = null
) : ICommand<ErrorOr<PromoCode>>;
