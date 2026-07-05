using cico.Application.Interfaces;
using cico.Application.Common.Interfaces.Persistence;
using cico.Domain.Entities;
using cico.Domain.Enums;
using cico.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace cico.Infrastructure.Jobs;

public class AttendanceSyncJob
{
    private readonly CICODbContext _db;
    private readonly IDeviceRepository _deviceRepo;
    private readonly IDeviceClientService _deviceClient;

    public AttendanceSyncJob(
        CICODbContext db,
        IDeviceRepository deviceRepo,
        IDeviceClientService deviceClient)
    {
        _db = db;
        _deviceRepo = deviceRepo;
        _deviceClient = deviceClient;
    }

    public async Task RunAsync()
    {
        var devices = await _deviceRepo.GetActiveDevicesAsync();
        if (devices.Count == 0) return;

        var employeeMap = await _db.Employees
            .Select(e => new { e.Id, e.EmployeeCode })
            .ToDictionaryAsync(e => e.EmployeeCode, e => e.Id);

        foreach (var device in devices)
        {
            if (string.IsNullOrWhiteSpace(device.IpAddress)) continue;

            var from = device.LastSyncTime ?? DateTime.UtcNow.AddDays(-1);
            var to = DateTime.UtcNow;

            try
            {
                var records = await _deviceClient.SearchAttendanceRecordsAsync(
                    device.IpAddress, device.Port,
                    device.Username ?? "admin",
                    device.Password ?? "admin",
                    from, to);

                if (records.Count == 0) continue;

                records = records.OrderBy(r => r.ScanTime).ToList();

                var recordCodes = records
                    .Select(r => r.EmployeeCode)
                    .Distinct()
                    .Where(employeeMap.ContainsKey)
                    .ToList();

                if (recordCodes.Count == 0) continue;

                var empIds = recordCodes.Select(c => employeeMap[c]).ToHashSet();
                var dates = records
                    .Select(r => DateOnly.FromDateTime(r.ScanTime))
                    .Distinct()
                    .ToHashSet();

                var existingAttendances = await _db.Attendances
                    .Where(a => empIds.Contains(a.EmployeeId) && dates.Contains(a.AttendanceDate))
                    .ToListAsync();

                var attendanceLookup = existingAttendances
                    .ToDictionary(a => (a.EmployeeId, a.AttendanceDate));

                var scheduleLookup = await _db.EmployeeSchedules
                    .Where(es => empIds.Contains(es.EmployeeId) && dates.Contains(es.WorkDate))
                    .Include(es => es.Schedule)
                    .ToDictionaryAsync(es => (es.EmployeeId, es.WorkDate));

                foreach (var record in records)
                {
                    if (!employeeMap.TryGetValue(record.EmployeeCode, out var empId)) continue;

                    var date = DateOnly.FromDateTime(record.ScanTime);
                    var key = (empId, date);

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
                            EmployeeId = empId,
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
                        Message = record.IsSuccess ? "Synced from device" : "Failed scan"
                    });
                }

                device.LastSyncTime = to;
                device.Status = DeviceStatus.Online;
            }
            catch
            {
                device.Status = DeviceStatus.Offline;
            }
        }

        await _db.SaveChangesAsync();
    }
}
