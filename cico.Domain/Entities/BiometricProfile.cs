using cico.Domain.Common;
using cico.Domain.Enums;

namespace cico.Domain.Entities;

public class BiometricProfile : AuditableEntity
{
    public Guid EmployeeId { get; set; }

    public Employee Employee { get; set; }
        = null!;

    public BiometricType Type { get; set; }

    public string Template { get; set; }
        = string.Empty;

    public bool IsActive { get; set; }
        = true;

    public DateTime? RegisteredAt { get; set; }

    public DateTime? LastVerifiedAt { get; set; }
}