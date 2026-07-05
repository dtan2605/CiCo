using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using cico.Application.Features.AttendanceLogs
    .Commands.CreateAttendanceLog;
using cico.Application.Features.AttendanceLogs
    .Queries.GetAttendanceLogById;
using cico.Application.Features.AttendanceLogs
    .Queries.GetAttendanceLogsByAttendance;
using cico.Application.Features.AttendanceLogs
    .Queries.GetAttendanceLogsByDevice;
using cico.Application.Features.AttendanceLogs
    .Queries.GetAttendanceLogsPaging;

using Microsoft.AspNetCore.RateLimiting;

namespace cico.API.Controllers;

[EnableRateLimiting("EmployeePolicy")]
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AttendanceLogController
    : ControllerBase
{
    private readonly IMediator _mediator;

    public AttendanceLogController(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetPaging(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] Guid? attendanceId = null,
        [FromQuery] Guid? deviceId = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] bool? isSuccess = null)
    {
        var result =
            await _mediator.Send(
                new GetAttendanceLogsPagingQuery(
                    pageNumber,
                    pageSize,
                    attendanceId,
                    deviceId,
                    fromDate,
                    toDate,
                    isSuccess));

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id)
    {
        var result =
            await _mediator.Send(
                new GetAttendanceLogByIdQuery(id));

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet("by-attendance/{attendanceId:guid}")]
    public async Task<IActionResult>
        GetByAttendance(Guid attendanceId)
    {
        var result =
            await _mediator.Send(
                new GetAttendanceLogsByAttendanceQuery(
                    attendanceId));

        return Ok(result);
    }

    [HttpGet("by-device/{deviceId:guid}")]
    public async Task<IActionResult>
        GetByDevice(Guid deviceId)
    {
        var result =
            await _mediator.Send(
                new GetAttendanceLogsByDeviceQuery(
                    deviceId));

        return Ok(result);
    }

    [HttpPost]
    [EnableRateLimiting("WritePolicy")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Create(
        CreateAttendanceLogCommand command)
    {
        var result =
            await _mediator.Send(command);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Id },
            result);
    }
}
