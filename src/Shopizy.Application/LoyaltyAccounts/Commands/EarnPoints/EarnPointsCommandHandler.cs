using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.LoyaltyAccounts;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.LoyaltyAccounts.Commands.EarnPoints;

public class EarnPointsCommandHandler(ILoyaltyAccountRepository loyaltyAccountRepository)
    : ICommandHandler<EarnPointsCommand, ErrorOr<LoyaltyAccount>>
{
    private readonly ILoyaltyAccountRepository _loyaltyAccountRepository = loyaltyAccountRepository;

    public async Task<ErrorOr<LoyaltyAccount>> Handle(
        EarnPointsCommand request,
        CancellationToken cancellationToken
    )
    {
        var userId = UserId.Create(request.UserId);
        var account = await _loyaltyAccountRepository.GetByUserIdAsync(userId);

        if (account is null)
        {
            account = LoyaltyAccount.Create(userId);
            await _loyaltyAccountRepository.AddAsync(account);
        }
        else
        {
            _loyaltyAccountRepository.Update(account);
        }

        account.EarnPoints(request.Points, request.Description);

        return account;
    }
}
