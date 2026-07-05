using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using cico.Domain.Entities;
using cico.Domain.Enums;
using cico.Infrastructure.Persistence;
using cico.Application.Interfaces;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.API.Controllers;

[Authorize(Policy = "AdminOnly")]
[ApiController]
[Route("api/devices")]
[EnableRateLimiting("EmployeePolicy")]
public class DevicesController : ControllerBase
{
    private readonly CICODbContext _db;
    private readonly IDeviceClientService _deviceClient;
    private readonly IDeviceRepository _deviceRepo;

    public DevicesController(
        CICODbContext db,
        IDeviceClientService deviceClient,
        IDeviceRepository deviceRepo)
    {
        _db = db;
        _deviceClient = deviceClient;
        _deviceRepo = deviceRepo;
    }

    [HttpGet]
    public async Task<IActionResult> GetPaging(
        [FromQuery] int page = 1,
        [FromQuery] int size = 20,
        [FromQuery] string? keyword = null)
    {
        var query = _db.Devices.AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
            query = query.Where(d =>
                d.Name.Contains(keyword) ||
                d.DeviceCode.Contains(keyword) ||
                d.IpAddress.Contains(keyword));

        var total = await query.CountAsync();
        var items = await query
            .OrderBy(d => d.Name)
            .Skip((page - 1) * size)
            .Take(size)
            .Select(d => new
            {
                d.Id,
                d.DeviceCode,
                d.Name,
                d.Location,
                d.IpAddress,
                d.Port,
                d.SerialNumber,
                d.Manufacturer,
                d.FirmwareVersion,
                d.LastSyncTime,
                Status = d.Status.ToString(),
                d.IsActive
            })
            .ToListAsync();

        return Ok(new { items, total, page, size });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var device = await _deviceRepo.GetByIdAsync(id);
        if (device == null) return NotFound();

        return Ok(new
        {
            device.Id,
            device.DeviceCode,
            device.Name,
            device.Location,
            device.IpAddress,
            device.Port,
            device.SerialNumber,
            device.Manufacturer,
            device.FirmwareVersion,
            device.Username,
            device.LastSyncTime,
            Status = device.Status.ToString(),
            device.IsActive
        });
    }

    [HttpPost]
    [EnableRateLimiting("WritePolicy")]
    public async Task<IActionResult> Create([FromBody] DeviceRequest request)
    {
        var existing = await _deviceRepo.GetByCodeAsync(request.DeviceCode);
        if (existing != null)
            return Conflict(new { error = $"Device code '{request.DeviceCode}' already exists" });

        var device = new Device
        {
            DeviceCode = request.DeviceCode,
            Name = request.Name,
            Location = request.Location ?? string.Empty,
            IpAddress = request.IpAddress,
            Port = request.Port > 0 ? request.Port : 80,
            SerialNumber = request.SerialNumber ?? string.Empty,
            Manufacturer = request.Manufacturer ?? "Hikvision",
            FirmwareVersion = request.FirmwareVersion ?? string.Empty,
            Username = request.Username,
            Password = request.Password,
            Status = DeviceStatus.Offline,
            IsActive = true
        };

        await _deviceRepo.AddAsync(device);

        return CreatedAtAction(nameof(GetById), new { id = device.Id }, device.Id);
    }

    [HttpPut("{id:guid}")]
    [EnableRateLimiting("WritePolicy")]
    public async Task<IActionResult> Update(Guid id, [FromBody] DeviceRequest request)
    {
        var device = await _deviceRepo.GetByIdAsync(id);
        if (device == null) return NotFound();

        var duplicate = await _deviceRepo.GetByCodeAsync(request.DeviceCode);
        if (duplicate != null && duplicate.Id != id)
            return Conflict(new { error = $"Device code '{request.DeviceCode}' is already in use" });

        device.DeviceCode = request.DeviceCode;
        device.Name = request.Name;
        device.Location = request.Location ?? string.Empty;
        device.IpAddress = request.IpAddress;
        device.Port = request.Port > 0 ? request.Port : 80;
        device.SerialNumber = request.SerialNumber ?? string.Empty;
        device.Manufacturer = request.Manufacturer ?? "Hikvision";
        device.FirmwareVersion = request.FirmwareVersion ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(request.Username))
            device.Username = request.Username;
        if (!string.IsNullOrWhiteSpace(request.Password))
            device.Password = request.Password;
        device.IsActive = request.IsActive;

        await _deviceRepo.UpdateAsync(device);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [EnableRateLimiting("DeletePolicy")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var device = await _deviceRepo.GetByIdAsync(id);
        if (device == null) return NotFound();

        try
        {
            await _deviceRepo.DeleteAsync(device);
            return NoContent();
        }
        catch (DbUpdateException)
        {
            return Conflict(new { error = "Cannot delete device with existing attendance logs. Deactivate it instead." });
        }
    }

    [HttpPost("{id:guid}/test-connection")]
    [EnableRateLimiting("WritePolicy")]
    public async Task<IActionResult> TestConnection(Guid id)
    {
        var device = await _deviceRepo.GetByIdAsync(id);
        if (device == null) return NotFound();

        if (string.IsNullOrWhiteSpace(device.IpAddress))
            return BadRequest(new { error = "Device IP address is not configured" });

        var result = await _deviceClient.TestConnectionAsync(
            device.IpAddress, device.Port,
            device.Username ?? "admin",
            device.Password ?? "admin");

        device.Status = result.Status;
        if (result.Success)
            device.LastSyncTime = DateTime.UtcNow;

        await _deviceRepo.UpdateAsync(device);

        return Ok(new { result.Success, result.Message, Status = result.Status.ToString() });
    }

    [HttpPost("sync-all")]
    [EnableRateLimiting("WritePolicy")]
    public async Task<IActionResult> SyncAll()
    {
        var devices = await _deviceRepo.GetActiveDevicesAsync();
        var employees = await _db.Employees
            .Select(e => new { e.Id, e.EmployeeCode })
            .ToListAsync();
        var employeeMap = employees.ToDictionary(e => e.EmployeeCode, e => e.Id);

        var results = new List<object>();

        foreach (var device in devices)
        {
            if (string.IsNullOrWhiteSpace(device.IpAddress))
            {
                results.Add(new { device.Id, device.Name, error = "IP address not configured" });
                continue;
            }

            var from = device.LastSyncTime ?? DateTime.UtcNow.AddDays(-1);
            var to = DateTime.UtcNow;

            try
            {
                var records = await _deviceClient.SearchAttendanceRecordsAsync(
                    device.IpAddress, device.Port,
                    device.Username ?? "admin",
                    device.Password ?? "admin",
                    from, to);

                if (records.Count == 0)
                {
                    device.LastSyncTime = to;
                    device.Status = DeviceStatus.Online;
                    results.Add(new { device.Id, device.Name, recordsFound = 0, processed = 0, error = (string?)null });
                    continue;
                }

                records = records.OrderBy(r => r.ScanTime).ToList();

                var codeMap = records
                    .Select(r => r.EmployeeCode)
                    .Distinct()
                    .Where(employeeMap.ContainsKey)
                    .Select(code => new { Code = code, EmployeeId = employeeMap[code] })
                    .ToList();

                var employeeIds = codeMap.Select(x => x.EmployeeId).ToHashSet();
                var recordDates = records
                    .Select(r => DateOnly.FromDateTime(r.ScanTime))
                    .Distinct()
                    .ToHashSet();

                var existingAttendances = await _db.Attendances
                    .Where(a => employeeIds.Contains(a.EmployeeId) && recordDates.Contains(a.AttendanceDate))
                    .ToListAsync();

                var attendanceLookup = existingAttendances
                    .ToDictionary(a => (a.EmployeeId, a.AttendanceDate));

                var scheduleLookup = await _db.EmployeeSchedules
                    .Where(es => employeeIds.Contains(es.EmployeeId) && recordDates.Contains(es.WorkDate))
                    .Include(es => es.Schedule)
                    .ToDictionaryAsync(es => (es.EmployeeId, es.WorkDate));

                var processed = 0;
                foreach (var record in records)
                {
                    if (!employeeMap.TryGetValue(record.EmployeeCode, out var employeeId)) continue;

                    var date = DateOnly.FromDateTime(record.ScanTime);
                    var key = (employeeId, date);

                    if (!attendanceLookup.TryGetValue(key, out var attendance))
                    {
                        var status = AttendanceStatus.Present;
                        var lateMinutes = 0;
                        Guid? empScheduleId = null;

                        if (scheduleLookup.TryGetValue(key, out var empSchedule))
                        {
                            empScheduleId = empSchedule.Id;
                            var checkInTimeOnly = TimeOnly.FromDateTime(record.ScanTime);
                            var diff = checkInTimeOnly - empSchedule.Schedule.StartTime;
                            if (diff.TotalMinutes > empSchedule.Schedule.LateThresholdMinutes)
                            {
                                lateMinutes = (int)diff.TotalMinutes;
                                status = AttendanceStatus.Late;
                            }
                        }

                        attendance = new Attendance
                        {
                            EmployeeId = employeeId,
                            AttendanceDate = date,
                            CheckInTime = record.ScanTime,
                            EmployeeScheduleId = empScheduleId,
                            Method = record.Method,
                            Status = status,
                            LateMinutes = lateMinutes
                        };
                        _db.Attendances.Add(attendance);
                        attendanceLookup[key] = attendance;
                    }
                    else if (attendance.CheckOutTime == null &&
                        record.ScanTime.TimeOfDay.TotalHours >= 12)
                    {
                        attendance.CheckOutTime = record.ScanTime;
                    }

                    _db.AttendanceLogs.Add(new AttendanceLog
                    {
                        AttendanceId = attendance.Id,
                        DeviceId = device.Id,
                        ScanTime = record.ScanTime,
                        Method = record.Method,
                        IsSuccess = record.IsSuccess,
                        Message = record.IsSuccess
                            ? "Synced from device"
                            : "Failed scan"
                    });

                    processed++;
                }

                device.LastSyncTime = to;
                device.Status = DeviceStatus.Online;

                results.Add(new
                {
                    device.Id,
                    device.Name,
                    recordsFound = records.Count,
                    processed,
                    error = (string?)null
                });
            }
            catch (Exception ex)
            {
                device.Status = DeviceStatus.Offline;
                results.Add(new
                {
                    device.Id,
                    device.Name,
                    recordsFound = 0,
                    processed = 0,
                    error = ex.Message
                });
            }
        }

        await _db.SaveChangesAsync();
        return Ok(new { synced = results.Count, details = results });
    }
}

public class DeviceRequest
{
    [Required(ErrorMessage = "Device code is required")]
    [MaxLength(50)]
    public string DeviceCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Device name is required")]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Location { get; set; }

    [Required(ErrorMessage = "IP address is required")]
    [MaxLength(50)]
    [RegularExpression(@"^\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}$",
        ErrorMessage = "Invalid IP address format")]
    public string IpAddress { get; set; } = string.Empty;

    [Range(1, 65535, ErrorMessage = "Port must be between 1 and 65535")]
    public int Port { get; set; } = 80;

    [MaxLength(100)]
    public string? SerialNumber { get; set; }

    [MaxLength(100)]
    public string? Manufacturer { get; set; }

    [MaxLength(50)]
    public string? FirmwareVersion { get; set; }

    [MaxLength(100)]
    public string? Username { get; set; }

    [MaxLength(500)]
    public string? Password { get; set; }

    public bool IsActive { get; set; } = true;
}
