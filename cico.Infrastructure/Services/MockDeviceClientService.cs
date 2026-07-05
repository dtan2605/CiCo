using cico.Application.Interfaces;
using cico.Domain.Enums;

namespace cico.Infrastructure.Services;

public class MockDeviceClientService : IDeviceClientService
{
    private readonly List<HikAttendanceRecord> _mockRecords;
    private readonly Random _rng = new();

    public MockDeviceClientService()
    {
        var employees = new[] { "ADM-001", "MGR-001", "EMP-001" };

        _mockRecords = employees
            .SelectMany(emp => Enumerable.Range(0, 12)
                .Select(dayOffset =>
                {
                    var day = DateTime.Today.AddDays(-dayOffset);
                    return new[]
                    {
                        new HikAttendanceRecord
                        {
                            EmployeeCode = emp,
                            ScanTime = day.AddHours(7).AddMinutes(_rng.Next(0, 90)),
                            Method = _rng.Next(2) == 0
                                ? AttendanceMethod.FaceRecognition
                                : AttendanceMethod.Fingerprint,
                            IsSuccess = true,
                            DeviceSerial = "MOCK-DEVICE-001"
                        },
                        new HikAttendanceRecord
                        {
                            EmployeeCode = emp,
                            ScanTime = day.AddHours(17).AddMinutes(_rng.Next(0, 60)),
                            Method = _rng.Next(2) == 0
                                ? AttendanceMethod.FaceRecognition
                                : AttendanceMethod.Fingerprint,
                            IsSuccess = _rng.NextDouble() > 0.1,
                            DeviceSerial = "MOCK-DEVICE-001"
                        }
                    };
                }))
            .SelectMany(x => x)
            .OrderByDescending(r => r.ScanTime)
            .ToList();
    }

    public Task<DeviceConnectionResult> TestConnectionAsync(
        string ip, int port, string username, string password)
    {
        return Task.FromResult(new DeviceConnectionResult
        {
            Success = true,
            Message = "Mock connection successful",
            Status = DeviceStatus.Online
        });
    }

    public Task<List<HikAttendanceRecord>> SearchAttendanceRecordsAsync(
        string ip, int port, string username, string password,
        DateTime from, DateTime to)
    {
        var results = _mockRecords
            .Where(r => r.ScanTime >= from && r.ScanTime <= to)
            .OrderBy(r => r.ScanTime)
            .Take(100)
            .ToList();

        return Task.FromResult(results);
    }

    public Task<bool> EnrollFaceAsync(
        string ip, int port, string username, string password,
        string employeeCode, byte[] faceImage)
    {
        return Task.FromResult(true);
    }

    public Task<bool> DeleteFaceAsync(
        string ip, int port, string username, string password,
        string employeeCode)
    {
        return Task.FromResult(true);
    }
}
