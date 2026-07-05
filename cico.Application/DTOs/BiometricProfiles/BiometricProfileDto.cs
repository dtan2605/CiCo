using cico.Domain.Enums;

namespace cico.Application.DTOs.BiometricProfiles;

public class BiometricProfileDto
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public string EmployeeName { get; set; }
        = string.Empty;

    public string EmployeeCode { get; set; }
        = string.Empty;

    public BiometricType Type { get; set; }

    public bool IsActive { get; set; }

    public DateTime? RegisteredAt { get; set; }

    public DateTime? LastVerifiedAt { get; set; }
}
