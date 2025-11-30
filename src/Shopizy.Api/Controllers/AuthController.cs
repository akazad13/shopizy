using ErrorOr;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Auth.Commands.Register;
using Shopizy.Application.Auth.Queries.login;
using Shopizy.Contracts.Authentication;
using Shopizy.Contracts.Common;
using Swashbuckle.AspNetCore.Annotations;

namespace Shopizy.Api.Controllers;

/// <summary>
/// Controller for handling user authentication and registration.
/// </summary>
[Route("api/v1.0/auth")]
public class AuthController(ISender mediator, IMapper mapper, ILogger<AuthController> logger)
    : ApiController
{
    private readonly ISender _mediator = mediator;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<AuthController> _logger = logger;

    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="request">The registration request containing user details.</param>
    /// <returns>A success result if registration is successful.</returns>
    /// <response code="200">If registration is successful.</response>
    /// <response code="400">If the request is invalid.</response>
    /// <response code="409">If the user already exists.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpPost("register")]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(SuccessResult))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status409Conflict, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(ErrorResult))]
    public async Task<IActionResult> RegisterAsync(RegisterRequest request)
    {
        try
        {
            var command = _mapper.Map<RegisterCommand>(request);
            var result = await _mediator.Send(command);

            return result.Match(
                success => Ok(SuccessResult.Success("Your account has been added. Please log in.")),
                Problem
            );
        }
        catch (Exception ex)
        {
            _logger.UserRegisterError(ex);
            return Problem([Error.Unexpected(description: ex.Message)]);
        }
    }

    /// <summary>
    /// Logs in a user.
    /// </summary>
    /// <param name="request">The login request containing credentials.</param>
    /// <returns>Authentication response with token.</returns>
    /// <response code="200">If login is successful.</response>
    /// <response code="400">If the request is invalid.</response>
    /// <response code="401">If credentials are invalid.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpPost("login")]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(AuthResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status409Conflict, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(ErrorResult))]
    public async Task<IActionResult> LoginAsync(LoginRequest request)
    {
        try
        {
            var query = _mapper.Map<LoginQuery>(request);
            var authResult = await _mediator.Send(query);

            return authResult.Match(
                authResult => Ok(_mapper.Map<AuthResponse>(authResult)),
                Problem
            );
        }
        catch (Exception ex)
        {
            _logger.UserLoginError(ex);
            return Problem([Error.Unexpected(description: ex.Message)]);
        }
    }
}
