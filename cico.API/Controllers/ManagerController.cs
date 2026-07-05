using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using cico.Infrastructure.Persistence;

namespace cico.API.Controllers;

[Authorize(Policy = "ManagerOnly")]
[ApiController]
[Route("api/manager")]
public class ManagerController : ControllerBase
{
    private readonly CICODbContext _db;

    public ManagerController(CICODbContext db)
    {
        _db = db;
    }

    private async Task<Guid?> GetDepartmentIdAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) return null;
        var emp = await _db.Employees
            .FirstOrDefaultAsync(e => e.UserId == Guid.Parse(userId));
        return emp?.DepartmentId;
    }

    [HttpGet("employees")]
    public async Task<IActionResult> GetEmployees()
    {
        var deptId = await GetDepartmentIdAsync();
        if (deptId == null) return NotFound("Employee record not found");

        var employees = await _db.Employees
            .Include(e => e.Department)
            .Include(e => e.Position)
            .Where(e => e.DepartmentId == deptId.Value && e.IsActive)
            .OrderBy(e => e.FullName)
            .Select(e => new
            {
                e.Id,
                e.EmployeeCode,
                e.FullName,
                e.Email,
                e.PhoneNumber,
                e.IsActive,
                DepartmentName = e.Department.Name,
                PositionName = e.Position.Name
            })
            .ToListAsync();

        return Ok(employees);
    }

    [HttpGet("attendance")]
    public async Task<IActionResult> GetAttendance(
        [FromQuery] DateOnly? fromDate,
        [FromQuery] DateOnly? toDate)
    {
        var deptId = await GetDepartmentIdAsync();
        if (deptId == null) return NotFound("Employee record not found");

        var query = _db.Attendances
            .Include(a => a.Employee)
            .Where(a => a.Employee.DepartmentId == deptId.Value);

        if (fromDate.HasValue)
            query = query.Where(a => a.AttendanceDate >= fromDate.Value);
        if (toDate.HasValue)
            query = query.Where(a => a.AttendanceDate <= toDate.Value);

        var records = await query
            .OrderByDescending(a => a.AttendanceDate)
            .Select(a => new
            {
                a.Id,
                a.AttendanceDate,
                a.CheckInTime,
                a.CheckOutTime,
                a.Status,
                a.LateMinutes,
                EmployeeName = a.Employee.FullName,
                EmployeeCode = a.Employee.EmployeeCode,
                a.EmployeeId
            })
            .ToListAsync();

        return Ok(records);
    }
}
