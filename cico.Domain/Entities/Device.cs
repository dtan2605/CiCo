using cico.Domain.Common;
using cico.Domain.Enums;

namespace cico.Domain.Entities;

public class Device : AuditableEntity
{
    public string DeviceCode { get; set; }
        = string.Empty;

    public string Name { get; set; }
        = string.Empty;

    public string Location { get; set; }
        = string.Empty;

    public string IpAddress { get; set; }
        = string.Empty;

    public int Port { get; set; } = 80;

    public string SerialNumber { get; set; }
        = string.Empty;

    public string Manufacturer { get; set; }
        = string.Empty;

    public string FirmwareVersion { get; set; }
        = string.Empty;

    public string? Username { get; set; }

    public string? Password { get; set; }

    public DateTime? LastSyncTime { get; set; }

    public DeviceStatus Status { get; set; }

    public bool IsActive { get; set; }
        = true;

    public ICollection<AttendanceLog> Logs
        { get; set; }
        = new List<AttendanceLog>();
}