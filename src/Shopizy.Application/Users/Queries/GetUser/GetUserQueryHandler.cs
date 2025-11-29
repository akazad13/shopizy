using ErrorOr;
using MediatR;
using Shopizy.Application.Common.Caching;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Orders.Enums;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Application.Users.Queries.GetUser;

/// <summary>
/// Handles the <see cref="GetUserQuery"/> to retrieve user information.
/// </summary>
public class GetUserQueryHandler(
    IUserRepository userRepository,
    IOrderRepository orderRepository,
    ICacheHelper cacheHelper
) : IRequestHandler<GetUserQuery, ErrorOr<UserDto>>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly ICacheHelper _cacheHelper = cacheHelper;

    /// <summary>
    /// Handles the query to retrieve user information including order statistics.
    /// </summary>
    /// <param name="request">The get user query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The user data transfer object or an error if the user is not found.</returns>
    public async Task<ErrorOr<UserDto>> Handle(
        GetUserQuery request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var cachedUser = await _cacheHelper.GetAsync<UserDto>($"user-{request.UserId}");
            if (cachedUser is not null)
            {
                return cachedUser;
            }
        }
        catch (Exception ex)
        {

        }

        try
        {
            var user = await _userRepository.GetUserById(UserId.Create(request.UserId));

            if (user is null)
            {
                return CustomErrors.User.UserNotFound;
            }

            var userOrders = _orderRepository
                .GetOrdersByUserId(user.Id)
                .Select(o => new { o.Id, o.OrderStatus })
                .ToList();

            var totalOrders = userOrders.Count;
            var totalRefundedOrders = userOrders.Count(o => o.OrderStatus == OrderStatus.Refunded);
            var totalFavorites = 0;

            var userDto = new UserDto(
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email,
                user.ProfileImageUrl,
                user.Phone,
                user.Address,
                totalOrders,
                user.ProductReviewIds.Count,
                totalFavorites,
                totalRefundedOrders,
                user.CreatedOn,
                user.ModifiedOn
            );

            await _cacheHelper.SetAsync($"user-{user.Id.Value}", userDto, TimeSpan.FromMinutes(60));

            return userDto;
        }
        catch (Exception ex)
        {
            return CustomErrors.User.UserNotFound;
        }
    }
}
