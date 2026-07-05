using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using cico.Application.Features.Employees.Commands.UpdateMyProfile;
using cico.Application.Features.Employees.Commands.UploadAvatar;

namespace cico.API.Controllers;

[Authorize]
[ApiController]
[Route("api/profile")]
public class ProfileController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProfileController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProfile(
        UpdateMyProfileCommand command)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var cmd = command with { UserId = Guid.Parse(userId) };
        await _mediator.Send(cmd);
        return NoContent();
    }

    [HttpPut("avatar")]
    public async Task<IActionResult> UploadAvatar(
        UploadAvatarCommand command)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var cmd = command with { UserId = Guid.Parse(userId) };
        var url = await _mediator.Send(cmd);
        return Ok(new { avatarUrl = url });
    }
}
