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
            return CustomErrors.PromoCode.PromoCodeNotFound;
        }

        promoCode.Update(
            request.Code,
            request.Description,
            request.Discount,
            request.IsPercentage,
            request.IsActive
        );

        _promoCodeRepository.Update(promoCode);

        return promoCode;
    }
}
