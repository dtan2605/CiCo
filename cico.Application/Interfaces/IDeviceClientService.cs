using cico.Domain.Enums;

namespace cico.Application.Interfaces;

public class HikAttendanceRecord
{
    public string EmployeeCode { get; set; } = string.Empty;
    public DateTime ScanTime { get; set; }
    public AttendanceMethod Method { get; set; }
    public bool IsSuccess { get; set; }
    public string? DeviceSerial { get; set; }
}

public class DeviceConnectionResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public DeviceStatus Status { get; set; }
}

public interface IDeviceClientService
{
    Task<DeviceConnectionResult> TestConnectionAsync(
        string ip, int port, string username, string password);

    Task<List<HikAttendanceRecord>> SearchAttendanceRecordsAsync(
        string ip, int port, string username, string password,
        DateTime from, DateTime to);

    Task<bool> EnrollFaceAsync(
        string ip, int port, string username, string password,
        string employeeCode, byte[] faceImage);

    Task<bool> DeleteFaceAsync(
        string ip, int port, string username, string password,
        string employeeCode);
}
