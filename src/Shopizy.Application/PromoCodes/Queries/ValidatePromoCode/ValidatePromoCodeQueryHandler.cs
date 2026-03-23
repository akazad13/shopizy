using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.PromoCodes;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.PromoCodes.Queries.ValidatePromoCode;

public class ValidatePromoCodeQueryHandler(IPromoCodeRepository promoCodeRepository)
    : IQueryHandler<ValidatePromoCodeQuery, ErrorOr<PromoCode>>
{
    private readonly IPromoCodeRepository _promoCodeRepository = promoCodeRepository;

    public async Task<ErrorOr<PromoCode>> Handle(
        ValidatePromoCodeQuery request,
        CancellationToken cancellationToken
    )
    {
        var promoCode = await _promoCodeRepository.GetByCodeAsync(request.Code);
        if (promoCode is null)
        {
            return CustomErrors.PromoCode.PromoCodeNotFound;
        }

        if (!promoCode.IsActive)
        {
            return CustomErrors.PromoCode.PromoCodeInactive;
        }

        return promoCode;
    }
}
