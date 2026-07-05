using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using cico.Application.Common.Interfaces.Persistence;
using cico.Application.Features.LeaveRequests.Commands.CreateLeaveRequest;
using cico.Application.Features.LeaveRequests.Commands.ResolveLeaveRequest;
using cico.Application.Features.LeaveRequests.Queries.GetPendingLeaveRequests;
using cico.Application.Features.LeaveRequests.Queries.GetMyLeaveRequests;
using cico.Domain.Enums;

namespace cico.API.Controllers;

[Authorize]
[ApiController]
[Route("api/leave-requests")]
public class LeaveRequestController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IEmployeeRepository _employeeRepo;

    public LeaveRequestController(
        IMediator mediator,
        IEmployeeRepository employeeRepo)
    {
        _mediator = mediator;
        _employeeRepo = employeeRepo;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateLeaveRequestCommand command)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var employee = await _employeeRepo.GetByUserIdAsync(Guid.Parse(userId));
        if (employee == null)
            return NotFound("Employee not found");

        var cmd = command with { EmployeeId = employee.Id };
        var id = await _mediator.Send(cmd);
        return Ok(new { id });
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMy()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var employee = await _employeeRepo.GetByUserIdAsync(Guid.Parse(userId));
        if (employee == null)
            return NotFound("Employee not found");

        var result = await _mediator.Send(
            new GetMyLeaveRequestsQuery(employee.Id));
        return Ok(result);
    }

    [HttpGet("pending")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetPending()
    {
        var result = await _mediator.Send(
            new GetPendingLeaveRequestsQuery());
        return Ok(result);
    }

    [HttpPut("{id:guid}/resolve")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Resolve(
        Guid id,
        [FromBody] ResolveLeaveRequestDto body)
    {
        var status = body.Approve
            ? LeaveStatus.Approved
            : LeaveStatus.Rejected;

        await _mediator.Send(
            new ResolveLeaveRequestCommand(id, status));

        return NoContent();
    }
}

public class ResolveLeaveRequestDto
{
    public bool Approve { get; set; }
}
