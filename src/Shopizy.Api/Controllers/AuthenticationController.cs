using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Application.Authentication.Commands.Register;
using Shopizy.Application.Authentication.Queries.login;
using Shopizy.Contracts.Authentication;

namespace Shopizy.Api.Controllers;

[Route("api/v1.0/auth")]
public class AuthenticationController(ISender mediator, IMapper mapper) : ApiController
{
    private readonly ISender _mediator = mediator;
    private readonly IMapper _mapper = mapper;

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync(RegisterRequest request)
    {
        RegisterCommand command = _mapper.Map<RegisterCommand>(request);
        ErrorOr.ErrorOr<Application.Authentication.Common.AuthResult> authResult =
            await _mediator.Send(command);

        return authResult.Match(authResult => Ok(_mapper.Map<AuthResponse>(authResult)), Problem);
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(LoginRequest request)
    {
        LoginQuery query = _mapper.Map<LoginQuery>(request);
        ErrorOr.ErrorOr<Application.Authentication.Common.AuthResult> authResult =
            await _mediator.Send(query);

        return authResult.Match(authResult => Ok(_mapper.Map<AuthResponse>(authResult)), Problem);
    }
}
