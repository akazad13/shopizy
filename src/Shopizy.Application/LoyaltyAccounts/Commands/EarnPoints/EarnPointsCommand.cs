using ErrorOr;
using Shopizy.Domain.LoyaltyAccounts;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.LoyaltyAccounts.Commands.EarnPoints;

public record EarnPointsCommand(Guid UserId, int Points, string Description) : ICommand<ErrorOr<LoyaltyAccount>>;
