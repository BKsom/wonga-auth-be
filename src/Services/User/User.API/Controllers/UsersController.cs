using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using User.Application.Commands.CreateUserProfile;
using User.Application.DTOs;
using User.Application.Queries.GetUserById;

namespace User.API.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator) => _mediator = mediator;

    /// <summary>Get current authenticated user's details</summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMe(CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var result = await _mediator.Send(new GetUserByIdQuery(userId), cancellationToken);
        if (result is null) return NotFound();
        return Ok(result);
    }

    /// <summary>Internal endpoint to create user profile (called by Auth Service)</summary>
    [HttpPost("internal/create")]
    [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateProfile([FromBody] CreateUserProfileCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetMe), result);
    }
}
