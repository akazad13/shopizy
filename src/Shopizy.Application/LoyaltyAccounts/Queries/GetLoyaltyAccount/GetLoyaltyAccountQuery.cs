using ErrorOr;
using Shopizy.Domain.LoyaltyAccounts;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.LoyaltyAccounts.Queries.GetLoyaltyAccount;

public record GetLoyaltyAccountQuery(Guid UserId) : IQuery<ErrorOr<LoyaltyAccount>>;
