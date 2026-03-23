using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.LoyaltyAccounts;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.LoyaltyAccounts.Commands.RedeemPoints;

public class RedeemPointsCommandHandler(ILoyaltyAccountRepository loyaltyAccountRepository)
    : ICommandHandler<RedeemPointsCommand, ErrorOr<LoyaltyAccount>>
{
    private readonly ILoyaltyAccountRepository _loyaltyAccountRepository = loyaltyAccountRepository;

    public async Task<ErrorOr<LoyaltyAccount>> Handle(
        RedeemPointsCommand request,
        CancellationToken cancellationToken
    )
    {
        var account = await _loyaltyAccountRepository.GetByUserIdAsync(UserId.Create(request.UserId));

        if (account is null)
        {
            return CustomErrors.LoyaltyAccount.AccountNotFound;
        }

        var result = account.RedeemPoints(request.Points, request.Description);
        if (result.IsError)
        {
            return result.Errors;
        }

        _loyaltyAccountRepository.Update(account);

        return account;
    }
}
