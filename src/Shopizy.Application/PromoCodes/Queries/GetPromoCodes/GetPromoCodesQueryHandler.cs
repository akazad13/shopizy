using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.PromoCodes;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.PromoCodes.Queries.GetPromoCodes;

public class GetPromoCodesQueryHandler(IPromoCodeRepository promoCodeRepository)
    : IQueryHandler<GetPromoCodesQuery, ErrorOr<List<PromoCode>>>
{
    private readonly IPromoCodeRepository _promoCodeRepository = promoCodeRepository;

    public async Task<ErrorOr<List<PromoCode>>> Handle(
        GetPromoCodesQuery request,
        CancellationToken cancellationToken
    )
    {
        var promoCodes = await _promoCodeRepository.GetPromoCodesAsync();
        return promoCodes.ToList();
    }
}
