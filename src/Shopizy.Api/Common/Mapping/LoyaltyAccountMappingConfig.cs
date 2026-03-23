using Mapster;
using Shopizy.Application.LoyaltyAccounts.Commands.EarnPoints;
using Shopizy.Application.LoyaltyAccounts.Commands.RedeemPoints;
using Shopizy.Contracts.LoyaltyAccount;
using Shopizy.Domain.LoyaltyAccounts;
using Shopizy.Domain.LoyaltyAccounts.Entities;

namespace Shopizy.Api.Common.Mapping;

public class LoyaltyAccountMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        ArgumentNullException.ThrowIfNull(config);

        config
            .NewConfig<(EarnPointsRequest request, Guid UserId), EarnPointsCommand>()
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest.Points, src => src.request.Points)
            .Map(dest => dest.Description, src => src.request.Description);

        config
            .NewConfig<(RedeemPointsRequest request, Guid UserId), RedeemPointsCommand>()
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest.Points, src => src.request.Points)
            .Map(dest => dest.Description, src => src.request.Description);

        config
            .NewConfig<LoyaltyTransaction, LoyaltyTransactionResponse>()
            .Map(dest => dest.TransactionId, src => src.Id.Value)
            .Map(dest => dest.Points, src => src.Points)
            .Map(dest => dest.Type, src => src.Type.ToString())
            .Map(dest => dest.Description, src => src.Description)
            .Map(dest => dest.CreatedOn, src => src.CreatedOn);

        config
            .NewConfig<LoyaltyAccount, LoyaltyAccountResponse>()
            .Map(dest => dest.AccountId, src => src.Id.Value)
            .Map(dest => dest.TotalPoints, src => src.TotalPoints)
            .Map(dest => dest.Transactions, src => src.Transactions);
    }
}
