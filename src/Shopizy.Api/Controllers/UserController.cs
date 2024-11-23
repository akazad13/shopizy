using ErrorOr;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Application.Users.Commands.UpdateAddress;
using Shopizy.Application.Users.Commands.UpdatePassword;
using Shopizy.Application.Users.Queries.GetUser;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.User;
using Swashbuckle.AspNetCore.Annotations;

namespace Shopizy.Api.Controllers;

[Route("api/v1.0/users/{userId:guid}")]
public class UserController(ISender mediator, IMapper mapper) : ApiController
{
    private readonly ISender _mediator = mediator;
    private readonly IMapper _mapper = mapper;

    [HttpGet]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(UserDetails))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(ErrorResult))]
    public async Task<IActionResult> GetUserAsync(Guid UserId)
    {
        var query = _mapper.Map<GetUserQuery>(UserId);
        var result = await _mediator.Send(query);

        return result.Match(user => Ok(_mapper.Map<UserDetails>(user)), Problem);
    }

    [HttpPatch("address")]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(Success))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status409Conflict, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(ErrorResult))]
    public async Task<IActionResult> UpdateUserAddressAsync(
        Guid userId,
        UpdateAddressRequest request
    )
    {
        var command = _mapper.Map<UpdateAddressCommand>((userId, request));
        var result = await _mediator.Send(command);

        return result.Match(
            success => Ok(SuccessResult.Success("Successfully updated address.")),
            Problem
        );
    }

    [HttpPatch("password")]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(Success))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status409Conflict, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(ErrorResult))]
    public async Task<IActionResult> UpdatePasswordAsync(Guid userId, UpdatePasswordRequest request)
    {
        var command = _mapper.Map<UpdatePasswordCommand>((userId, request));
        var result = await _mediator.Send(command);

        return result.Match<IActionResult>(
            success => Ok(SuccessResult.Success("Successfully updated password.")),
            Problem
        );
    }
}
