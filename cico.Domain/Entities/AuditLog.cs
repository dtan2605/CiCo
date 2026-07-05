using cico.Domain.Common;

namespace cico.Domain.Entities;

public class AuditLog : AuditableEntity
{
    public Guid? UserId { get; set; }

    public User? User { get; set; }

    public string Action { get; set; }
        = string.Empty;

    public string EntityName { get; set; }
        = string.Empty;

    public Guid EntityId { get; set; }

    public string? OldValues { get; set; }

    public string? NewValues { get; set; }

    public string? IpAddress { get; set; }

    public string? UserAgent { get; set; }

    public DateTime ActionTime { get; set; }
        = DateTime.UtcNow;
}