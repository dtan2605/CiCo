using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using cico.Application.Features.Notifications.Commands.MarkAsRead;
using cico.Application.Features.Notifications.Commands.MarkAllAsRead;
using cico.Application.Features.Notifications.Commands.CreateNotification;
using cico.Application.Features.Notifications.Commands.DeleteNotification;
using cico.Application.Features.Notifications.Queries.GetUnread;
using cico.Application.Features.Notifications.Queries.GetNotificationsPaging;
using cico.Domain.Enums;

using Microsoft.AspNetCore.RateLimiting;

namespace cico.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotificationController(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetPaging(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] bool? isRead = null,
        [FromQuery] NotificationType? type = null)
    {
        var userId = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var result =
            await _mediator.Send(
                new GetNotificationsPagingQuery(
                    Guid.Parse(userId),
                    pageNumber,
                    pageSize,
                    isRead,
                    type));

        return Ok(result);
    }

    [HttpGet("unread")]
    public async Task<IActionResult> GetUnread()
    {
        var userId = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var result =
            await _mediator.Send(
                new GetUnreadQuery(
                    Guid.Parse(userId)));

        return Ok(result);
    }

    [HttpPost]
    [EnableRateLimiting("WritePolicy")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Create(
        CreateNotificationCommand command)
    {
        var result =
            await _mediator.Send(command);

        return CreatedAtAction(
            nameof(GetPaging),
            new { id = result.Id },
            result);
    }

    [HttpPut("{id:guid}/read")]
    public async Task<IActionResult> MarkAsRead(
        Guid id)
    {
        await _mediator.Send(
            new MarkAsReadCommand(id));

        return NoContent();
    }

    [HttpPut("read-all")]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var userId = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        await _mediator.Send(
            new MarkAllAsReadCommand(
                Guid.Parse(userId)));

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [EnableRateLimiting("WritePolicy")]
    public async Task<IActionResult> Delete(
        Guid id)
    {
        await _mediator.Send(
            new DeleteNotificationCommand(id));

        return NoContent();
    }
}
