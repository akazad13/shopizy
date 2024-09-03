using ErrorOr;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Application.Users.Commands.UpdateAddress;
using Shopizy.Application.Users.Commands.UpdatePassword;
using Shopizy.Application.Users.Queries.GetUser;
using Shopizy.Contracts.User;

namespace Shopizy.Api.Controllers;

[Route("api/v1.0/users/{userId:guid}")]
public class UserController(ISender mediator, IMapper mapper) : ApiController
{
    private readonly ISender _mediator = mediator;
    private readonly IMapper _mapper = mapper;

    [HttpGet]
    public async Task<IActionResult> GetUserAsync(Guid UserId)
    {
        GetUserQuery query = _mapper.Map<GetUserQuery>(UserId);
        ErrorOr<Domain.Users.User> result = await _mediator.Send(query);

        return result.Match(user => Ok(_mapper.Map<UserDetails>(user)), Problem);
    }

    [HttpPatch("address")]
    public async Task<IActionResult> UpdateUserAddressAsync(
        Guid userId,
        UpdateAddressRequest request
    )
    {
        UpdateAddressCommand command = _mapper.Map<UpdateAddressCommand>((userId, request));
        ErrorOr<Success> result = await _mediator.Send(command);

        return result.Match(orderId => Ok(_mapper.Map<Success>(orderId)), Problem);
    }

    [HttpPatch("password")]
    public async Task<IActionResult> UpdatePasswordAsync(Guid userId, UpdatePasswordRequest request)
    {
        UpdatePasswordCommand command = _mapper.Map<UpdatePasswordCommand>((userId, request));
        ErrorOr<Success> result = await _mediator.Send(command);

        return result.Match(orderId => Ok(_mapper.Map<Success>(orderId)), Problem);
    }
}
