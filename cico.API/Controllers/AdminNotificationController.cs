using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using cico.Application.Features.Notifications.Commands.CreateBroadcastNotification;

namespace cico.API.Controllers;

[Authorize(Policy = "AdminOnly")]
[ApiController]
[Route("api/admin/notifications")]
public class AdminNotificationController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminNotificationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("broadcast")]
    public async Task<IActionResult> Broadcast(CreateBroadcastNotificationCommand command)
    {
        await _mediator.Send(command);
        return Ok();
    }
}
