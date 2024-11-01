using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Application.Common.Wrappers;
using Shopizy.Application.Users.Commands.UpdateAddress;
using Shopizy.Application.Users.Commands.UpdatePassword;
using Shopizy.Application.Users.Queries.GetUser;
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
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(GenericResponse))]
    public async Task<IActionResult> GetUserAsync(Guid UserId)
    {
        var query = _mapper.Map<GetUserQuery>(UserId);
        var result = await _mediator.Send(query);

        return result.Match<IActionResult>(user => Ok(_mapper.Map<UserDetails>(user)), BadRequest);
    }

    [HttpPatch("address")]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status409Conflict, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(GenericResponse))]
    public async Task<IActionResult> UpdateUserAddressAsync(
        Guid userId,
        UpdateAddressRequest request
    )
    {
        var command = _mapper.Map<UpdateAddressCommand>((userId, request));
        var result = await _mediator.Send(command);

        return result.Match<IActionResult>(
            orderId => Ok(_mapper.Map<GenericResponse>(orderId)),
            BadRequest
        );
    }

    [HttpPatch("password")]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status409Conflict, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(GenericResponse))]
    public async Task<IActionResult> UpdatePasswordAsync(Guid userId, UpdatePasswordRequest request)
    {
        var command = _mapper.Map<UpdatePasswordCommand>((userId, request));
        var result = await _mediator.Send(command);

        return result.Match<IActionResult>(
            orderId => Ok(_mapper.Map<GenericResponse>(orderId)),
            BadRequest
        );
    }
}
