using ErrorOr;
using Shopizy.Domain.LoyaltyAccounts;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.LoyaltyAccounts.Commands.RedeemPoints;

public record RedeemPointsCommand(Guid UserId, int Points, string Description) : ICommand<ErrorOr<LoyaltyAccount>>;
