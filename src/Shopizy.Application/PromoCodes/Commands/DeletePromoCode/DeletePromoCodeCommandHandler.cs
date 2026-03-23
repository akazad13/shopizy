using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.PromoCodes.ValueObjects;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.PromoCodes.Commands.DeletePromoCode;

public class DeletePromoCodeCommandHandler(IPromoCodeRepository promoCodeRepository)
    : ICommandHandler<DeletePromoCodeCommand, ErrorOr<Deleted>>
{
    private readonly IPromoCodeRepository _promoCodeRepository = promoCodeRepository;

    public async Task<ErrorOr<Deleted>> Handle(
        DeletePromoCodeCommand request,
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

        _promoCodeRepository.Remove(promoCode);

        return Result.Deleted;
    }
}
