using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.LoyaltyAccounts;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.LoyaltyAccounts.Queries.GetLoyaltyAccount;

public class GetLoyaltyAccountQueryHandler(ILoyaltyAccountRepository loyaltyAccountRepository)
    : IQueryHandler<GetLoyaltyAccountQuery, ErrorOr<LoyaltyAccount>>
{
    private readonly ILoyaltyAccountRepository _loyaltyAccountRepository = loyaltyAccountRepository;

    public async Task<ErrorOr<LoyaltyAccount>> Handle(
        GetLoyaltyAccountQuery request,
        CancellationToken cancellationToken
    )
    {
        var account = await _loyaltyAccountRepository.GetByUserIdAsync(UserId.Create(request.UserId));

        if (account is null)
        {
            return CustomErrors.LoyaltyAccount.AccountNotFound;
        }

        return account;
    }
}
