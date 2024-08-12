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
public class UserController(ISender _mediator, IMapper _mapper) : ApiController
{
    [HttpGet]
    public async Task<IActionResult> GetUser(Guid UserId)
    {
        var query = _mapper.Map<GetUserQuery>(UserId);
        var result = await _mediator.Send(query);

        return result.Match(
            user => Ok(_mapper.Map<UserDetails>(user)),
            Problem);
    }

    [HttpPatch("address")]
    public async Task<IActionResult> UpdateUserAddress(Guid userId, UpdateAddressRequest request)
    {
        var command = _mapper.Map<UpdateAddressCommand>((userId, request));
        var result = await _mediator.Send(command);

        return result.Match(
            orderId => Ok(_mapper.Map<Success>(orderId)),
            Problem);
    }

    [HttpPatch("password")]
    public async Task<IActionResult> UpdatePassword(Guid userId, UpdatePasswordRequest request)
    {
        var command = _mapper.Map<UpdatePasswordCommand>((userId, request));
        var result = await _mediator.Send(command);

        return result.Match(
            orderId => Ok(_mapper.Map<Success>(orderId)),
            Problem);
    }
}