using cico.Domain.Common;
using cico.Domain.Enums;

namespace cico.Domain.Entities;

public class ProfileUpdateRequest : BaseEntity
{
    public Guid EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;

    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;

    public ProfileUpdateStatus Status { get; set; } = ProfileUpdateStatus.Pending;

    public Guid? ResolvedByUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvedAt { get; set; }
}
