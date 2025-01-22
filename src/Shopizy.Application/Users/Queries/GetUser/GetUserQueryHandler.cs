using ErrorOr;
using MediatR;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Orders.Enums;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Application.Users.Queries.GetUser;

public class GetUserQueryHandler(IUserRepository userRepository, IOrderRepository orderRepository)
    : IRequestHandler<GetUserQuery, ErrorOr<UserDto>>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IOrderRepository _orderRepository = orderRepository;

    public async Task<ErrorOr<UserDto>> Handle(
        GetUserQuery request,
        CancellationToken cancellationToken
    )
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

        return userDto;
    }
}
