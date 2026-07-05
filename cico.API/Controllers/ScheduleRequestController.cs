using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using cico.Application.Features.ScheduleRequests.Commands;
using cico.Application.Features.ScheduleRequests.Queries;
using Microsoft.AspNetCore.RateLimiting;

namespace cico.API.Controllers;

[Authorize]
[ApiController]
[Route("api/schedule-requests")]
[EnableRateLimiting("EmployeePolicy")]
public class ScheduleRequestController : ControllerBase
{
    private readonly IMediator _mediator;

    public ScheduleRequestController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetPaging(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] Guid? employeeId = null,
        [FromQuery] int? status = null)
    {
        var result = await _mediator.Send(
            new GetScheduleRequestsQuery(pageNumber, pageSize, employeeId, status));
        return Ok(result);
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyRequests(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

        var result = await _mediator.Send(
            new GetScheduleRequestsQuery(pageNumber, pageSize, Guid.Parse(userId)));
        return Ok(result);
    }

    [HttpPost]
    [EnableRateLimiting("WritePolicy")]
    public async Task<IActionResult> Create(CreateScheduleRequestCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(id);
    }

    [HttpPut("{id:guid}/resolve")]
    [EnableRateLimiting("WritePolicy")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Resolve(Guid id, ResolveScheduleRequestCommand command)
    {
        if (id != command.Id) return BadRequest("Id mismatch");
        await _mediator.Send(command);
        return NoContent();
    }
}
