using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using cico.Application.Features.Attendance.Commands.CheckIn;
using cico.Application.Features.Attendance.Commands.CheckOut;
using cico.Application.Features.Attendance.Commands.CancelAttendance;
using cico.Application.Features.Attendance.Queries.GetAttendanceById;
using cico.Application.Features.Attendance.Queries.GetAttendanceByEmployee;
using cico.Application.Features.Attendance.Queries.GetAttendanceByDate;

using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

namespace cico.API.Controllers;

[EnableRateLimiting("EmployeePolicy")]
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AttendanceController : ControllerBase
{
    private readonly IMediator _mediator;

    public AttendanceController(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id)
    {
        var result =
            await _mediator.Send(
                new GetAttendanceByIdQuery(id));

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet("by-employee/{employeeId:guid}")]
    public async Task<IActionResult> GetByEmployee(
        Guid employeeId)
    {
        var result =
            await _mediator.Send(
                new GetAttendanceByEmployeeQuery(
                    employeeId));

        return Ok(result);
    }

    [HttpGet("by-date")]
    public async Task<IActionResult> GetByDate(
        [FromQuery] DateOnly date)
    {
        var result =
            await _mediator.Send(
                new GetAttendanceByDateQuery(date));

        return Ok(result);
    }

    [HttpPost("check-in")]
    [EnableRateLimiting("WritePolicy")]
    [Authorize]
    public async Task<IActionResult> CheckIn(
        CheckInCommand command)
    {
        var result =
            await _mediator.Send(command);

        return Ok(result);
    }

    [HttpPut("check-out")]
    [EnableRateLimiting("WritePolicy")]
    [Authorize]
    public async Task<IActionResult> CheckOut(
        CheckOutCommand command)
    {
        await _mediator.Send(command);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Cancel(
        Guid id,
        [FromBody] CancelAttendanceCommand command)
    {
        if (id != command.AttendanceId)
            return BadRequest("Id mismatch");
        await _mediator.Send(command);
        return NoContent();
    }
}

[Authorize(Policy = "AdminOnly")]
[ApiController]
[Route("api/admin/attendance")]
public class AdminAttendanceController : ControllerBase
{
    private readonly cico.Infrastructure.Persistence.CICODbContext _db;

    public AdminAttendanceController(cico.Infrastructure.Persistence.CICODbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] DateOnly? fromDate,
        [FromQuery] DateOnly? toDate,
        [FromQuery] Guid? employeeId)
    {
        var query = _db.Attendances
            .Include(a => a.Employee)
            .AsQueryable();

        if (fromDate.HasValue)
            query = query.Where(a => a.AttendanceDate >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(a => a.AttendanceDate <= toDate.Value);

        if (employeeId.HasValue)
            query = query.Where(a => a.EmployeeId == employeeId.Value);

        var list = await query
            .OrderByDescending(a => a.AttendanceDate)
            .ThenBy(a => a.Employee.FullName)
            .ToListAsync();

        return Ok(list.Select(a => new
        {
            a.Id,
            a.AttendanceDate,
            a.CheckInTime,
            a.CheckOutTime,
            a.LateMinutes,
            a.Status,
            a.EmployeeId,
            EmployeeName = a.Employee.FullName,
            EmployeeCode = a.Employee.EmployeeCode
        }));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var attendance = await _db.Attendances.FindAsync(id);
        if (attendance == null) return NotFound();
        _db.Attendances.Remove(attendance);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
