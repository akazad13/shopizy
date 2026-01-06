using ErrorOr;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Users.Commands.UpdateAddress;
using Shopizy.Application.Users.Commands.UpdatePassword;
using Shopizy.Application.Users.Commands.UpdateUser;
using Shopizy.Application.Users.Queries.GetUser;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.User;
using Swashbuckle.AspNetCore.Annotations;

namespace Shopizy.Api.Controllers;

/// <summary>
/// Controller for managing user profiles.
/// </summary>
[Authorize]
[Route("api/v1.0/users/{userId:guid}")]
public class UserController(ISender mediator, IMapper mapper, ILogger<UserController> logger)
    : ApiController
{
    private readonly ISender _mediator = mediator;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<UserController> _logger = logger;

    /// <summary>
    /// Retrieves user details.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>The user details.</returns>
    /// <response code="200">Returns the user details.</response>
    /// <response code="400">If the request is invalid.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpGet]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(UserDetails))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(ErrorResult))]
    public async Task<IActionResult> GetUserAsync(Guid userId)
    {
        try
        {
            var query = _mapper.Map<GetUserQuery>(userId);
            var result = await _mediator.Send(query);

            return result.Match(user => Ok(_mapper.Map<UserDetails>(user)), Problem);
        }
        catch (Exception ex)
        {
            _logger.UserFetchError(ex);
            return Problem([Error.Unexpected(description: ex.Message)]);
        }
    }

    /// <summary>
    /// Updates user profile information.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="request">The update request.</param>
    /// <returns>Success result.</returns>
    /// <response code="200">If update is successful.</response>
    /// <response code="400">If the request is invalid.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpPut]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(Success))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status409Conflict, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(ErrorResult))]
    public async Task<IActionResult> UpdateUserAsync(Guid userId, UpdateUserRequest request)
    {
        try
        {
            var command = _mapper.Map<UpdateUserCommand>((userId, request));
            var result = await _mediator.Send(command);

            return result.Match(
                success => Ok(SuccessResult.Success("Successfully updated user.")),
                Problem
            );
        }
        catch (Exception ex)
        {
            _logger.UserUpdateError(ex);
            return Problem([Error.Unexpected(description: ex.Message)]);
        }
    }

    /// <summary>
    /// Updates user address.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="request">The address update request.</param>
    /// <returns>Success result.</returns>
    /// <response code="200">If update is successful.</response>
    /// <response code="400">If the request is invalid.</response>
    /// <response code="500">If an internal server error occurs.</response>
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
        try
        {
            var command = _mapper.Map<UpdateAddressCommand>((userId, request));
            var result = await _mediator.Send(command);

            return result.Match(
                success => Ok(SuccessResult.Success("Successfully updated address.")),
                Problem
            );
        }
        catch (Exception ex)
        {
            _logger.UserAddressUpdateError(ex);
            return Problem([Error.Unexpected(description: ex.Message)]);
        }
    }

    /// <summary>
    /// Updates user password.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="request">The password update request.</param>
    /// <returns>Success result.</returns>
    /// <response code="200">If update is successful.</response>
    /// <response code="400">If the request is invalid.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpPatch("password")]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(Success))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status409Conflict, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(ErrorResult))]
    public async Task<IActionResult> UpdatePasswordAsync(Guid userId, UpdatePasswordRequest request)
    {
        try
        {
            var command = _mapper.Map<UpdatePasswordCommand>((userId, request));
            var result = await _mediator.Send(command);

            return result.Match(
                success => Ok(SuccessResult.Success("Successfully updated password.")),
                Problem
            );
        }
        catch (Exception ex)
        {
            _logger.UserPasswordUpdateError(ex);
            return Problem([Error.Unexpected(description: ex.Message)]);
        }
    }
}
