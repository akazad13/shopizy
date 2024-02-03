using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Application.Authentication.Queries.login;
using Shopizy.Contracts.Authentication;

namespace Shopizy.Api.Controllers;

[Route("auth")]
public class AuthenticationController(ISender _mediator, IMapper _mapper) : ApiController
{
    [HttpPost("register")]
    public IActionResult Register(RegisterRequest request)
    {
        return Ok(request);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var query = _mapper.Map<LoginQuery>(request);
        var authResult = await _mediator.Send(query);

        return authResult.Match(
            authResult => Ok(_mapper.Map<AuthResponse>(authResult)),
            errors => Problem(errors));
    }
}
