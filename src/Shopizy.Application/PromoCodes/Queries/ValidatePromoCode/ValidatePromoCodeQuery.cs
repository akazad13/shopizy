using ErrorOr;
using Shopizy.Domain.PromoCodes;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.PromoCodes.Queries.ValidatePromoCode;

public record ValidatePromoCodeQuery(string Code) : IQuery<ErrorOr<PromoCode>>;
