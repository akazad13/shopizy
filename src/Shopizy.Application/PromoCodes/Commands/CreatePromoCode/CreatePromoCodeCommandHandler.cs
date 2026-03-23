using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.PromoCodes;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.PromoCodes.Commands.CreatePromoCode;

public class CreatePromoCodeCommandHandler(IPromoCodeRepository promoCodeRepository)
    : ICommandHandler<CreatePromoCodeCommand, ErrorOr<PromoCode>>
{
    private readonly IPromoCodeRepository _promoCodeRepository = promoCodeRepository;

    public async Task<ErrorOr<PromoCode>> Handle(
        CreatePromoCodeCommand request,
        CancellationToken cancellationToken
    )
    {
        var existing = await _promoCodeRepository.GetByCodeAsync(request.Code);
        if (existing is not null)
        {
            return CustomErrors.PromoCode.DuplicateCode;
        }

        var promoCode = PromoCode.Create(
            request.Code,
            request.Description,
            request.Discount,
            request.IsPercentage,
            request.IsActive
        );

        await _promoCodeRepository.AddAsync(promoCode);

        return promoCode;
    }
}
