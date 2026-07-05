using Microsoft.AspNetCore.Mvc;
using cico.Application.Interfaces;
using cico.Domain.Enums;
using cico.Domain.Entities;
using cico.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace cico.API.Controllers;

[ApiController]
[Route("api/webhook/hikvision")]
public class DeviceWebhookController : ControllerBase
{
    private readonly CICODbContext _db;

    public DeviceWebhookController(CICODbContext db)
    {
        _db = db;
    }

    [HttpPost("event")]
    public async Task<IActionResult> ReceiveEvent([FromBody] HikvisionPushEvent payload)
    {
        var device = await _db.Devices
            .FirstOrDefaultAsync(d => d.SerialNumber == payload.SerialNumber);
        if (device == null)
            return Ok(new { status = "ignored", reason = "Unknown device" });

        var employee = await _db.Employees
            .FirstOrDefaultAsync(e => e.EmployeeCode == payload.EmployeeCode);
        if (employee == null)
            return Ok(new { status = "ignored", reason = "Unknown employee" });

        var date = DateOnly.FromDateTime(payload.ScanTime);
        var attendance = await _db.Attendances
            .FirstOrDefaultAsync(a => a.EmployeeId == employee.Id && a.AttendanceDate == date);

        if (attendance == null)
        {
            var status = AttendanceStatus.Present;
            var lateMinutes = 0;
            Guid? empScheduleId = null;

            var empSchedule = await _db.EmployeeSchedules
                .Include(es => es.Schedule)
                .FirstOrDefaultAsync(es =>
                    es.EmployeeId == employee.Id && es.WorkDate == date);

            if (empSchedule != null)
            {
                empScheduleId = empSchedule.Id;
                var checkInTimeOnly = TimeOnly.FromDateTime(payload.ScanTime);
                var diff = checkInTimeOnly - empSchedule.Schedule.StartTime;
                if (diff.TotalMinutes > empSchedule.Schedule.LateThresholdMinutes)
                {
                    lateMinutes = (int)diff.TotalMinutes;
                    status = AttendanceStatus.Late;
                }
            }

            attendance = new Attendance
            {
                EmployeeId = employee.Id,
                AttendanceDate = date,
                CheckInTime = payload.ScanTime,
                EmployeeScheduleId = empScheduleId,
                Method = payload.Method,
                Status = status,
                LateMinutes = lateMinutes
            };
            _db.Attendances.Add(attendance);
        }
        else if (attendance.CheckOutTime == null &&
                 payload.ScanTime.TimeOfDay.TotalHours >= 12)
        {
            attendance.CheckOutTime = payload.ScanTime;
        }

        _db.AttendanceLogs.Add(new AttendanceLog
        {
            AttendanceId = attendance.Id,
            DeviceId = device.Id,
            ScanTime = payload.ScanTime,
            Method = payload.Method,
            IsSuccess = true,
            Message = "Real-time push from device"
        });

        device.LastSyncTime = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok(new { status = "ok" });
    }
}

public class HikvisionPushEvent
{
    public string SerialNumber { get; set; } = string.Empty;
    public string EmployeeCode { get; set; } = string.Empty;
    public DateTime ScanTime { get; set; }
    public AttendanceMethod Method { get; set; }
}
