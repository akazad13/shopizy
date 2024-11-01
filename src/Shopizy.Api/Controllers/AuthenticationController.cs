using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Application.Authentication.Commands.Register;
using Shopizy.Application.Authentication.Queries.login;
using Shopizy.Application.Common.Wrappers;
using Shopizy.Contracts.Authentication;
using Swashbuckle.AspNetCore.Annotations;

namespace Shopizy.Api.Controllers;

[Route("api/v1.0/auth")]
public class AuthenticationController(ISender mediator, IMapper mapper) : ApiController
{
    private readonly ISender _mediator = mediator;
    private readonly IMapper _mapper = mapper;

    [HttpPost("register")]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(AuthResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status409Conflict, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(GenericResponse))]
    public async Task<IActionResult> RegisterAsync(RegisterRequest request)
    {
        var command = _mapper.Map<RegisterCommand>(request);
        var authResult = await _mediator.Send(command);

        return authResult.Match<IActionResult>(
            authResult => Ok(_mapper.Map<AuthResponse>(authResult)),
            BadRequest
        );
    }

    [HttpPost("login")]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(AuthResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status409Conflict, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(GenericResponse))]
    public async Task<IActionResult> LoginAsync(LoginRequest request)
    {
        var query = _mapper.Map<LoginQuery>(request);
        var authResult = await _mediator.Send(query);

        return authResult.Match<IActionResult>(
            authResult => Ok(_mapper.Map<AuthResponse>(authResult)),
            BadRequest
        );
    }
}
