using ErrorOr;
using Shopizy.Domain.PromoCodes;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.PromoCodes.Queries.GetPromoCodes;

public record GetPromoCodesQuery : IQuery<ErrorOr<List<PromoCode>>>;
