using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.PromoCodes;
using Shopizy.Domain.PromoCodes.ValueObjects;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.PromoCodes.Commands.UpdatePromoCode;

public class UpdatePromoCodeCommandHandler(IPromoCodeRepository promoCodeRepository)
    : ICommandHandler<UpdatePromoCodeCommand, ErrorOr<PromoCode>>
{
    private readonly IPromoCodeRepository _promoCodeRepository = promoCodeRepository;

    public async Task<ErrorOr<PromoCode>> Handle(
        UpdatePromoCodeCommand request,
        CancellationToken cancellationToken
    )
    {
        var promoCode = await _promoCodeRepository.GetPromoCodeByIdAsync(
            PromoCodeId.Create(request.PromoCodeId)
        );
        if (promoCode is null)
        {
            return (Error)CustomErrors.PromoCode.PromoCodeNotFound;
        }

        promoCode.Update(
            request.Code,
            request.Description,
            request.Discount,
            request.IsPercentage,
            request.IsActive,
            request.PromoType,
            request.MinimumOrderAmount,
            request.MaxDiscountAmount,
            request.TargetCategoryId.HasValue
                ? Shopizy.Domain.Categories.ValueObjects.CategoryId.Create(
                    request.TargetCategoryId.Value
                )
                : null,
            request.BuyQuantity,
            request.GetQuantity,
            request.GetDiscountPercentage,
            request.UsageLimit,
            request.StartDate,
            request.EndDate
        );

        return promoCode;
    }
}
