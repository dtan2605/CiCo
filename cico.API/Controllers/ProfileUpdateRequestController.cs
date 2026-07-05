using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using cico.Application.Common.Interfaces.Persistence;
using cico.Application.Features.ProfileUpdateRequests.Commands.CreateProfileUpdateRequest;
using cico.Application.Features.ProfileUpdateRequests.Commands.ResolveProfileUpdateRequest;
using cico.Application.Features.ProfileUpdateRequests.Queries.GetPendingProfileUpdateRequests;
using cico.Application.Features.ProfileUpdateRequests.Queries.GetMyProfileUpdateRequests;
using cico.Application.DTOs.ProfileUpdateRequests;
using cico.Domain.Enums;

namespace cico.API.Controllers;

[Authorize]
[ApiController]
[Route("api/profile-update-requests")]
public class ProfileUpdateRequestController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IEmployeeRepository _employeeRepo;

    public ProfileUpdateRequestController(
        IMediator mediator,
        IEmployeeRepository employeeRepo)
    {
        _mediator = mediator;
        _employeeRepo = employeeRepo;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateProfileUpdateRequestCommand command)
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
            new GetMyProfileUpdateRequestsQuery(employee.Id));
        return Ok(result);
    }

    [HttpGet("pending")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetPending()
    {
        var result = await _mediator.Send(
            new GetPendingProfileUpdateRequestsQuery());
        return Ok(result);
    }

    [HttpPut("{id:guid}/resolve")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Resolve(
        Guid id,
        [FromBody] ResolveRequest body)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var status = body.Approve
            ? ProfileUpdateStatus.Approved
            : ProfileUpdateStatus.Rejected;

        await _mediator.Send(
            new ResolveProfileUpdateRequestCommand(
                id, status, Guid.Parse(userId)));

        return NoContent();
    }
}

public class ResolveRequest
{
    public bool Approve { get; set; }
}
