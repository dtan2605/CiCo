using cico.Domain.Common;
using cico.Domain.Enums;
using cico.Domain.ValueObjects;

namespace cico.Domain.Entities;
public class Employee : AuditableEntity
{
    public string EmployeeCode { get; set; }
        = string.Empty;

    public string FullName { get; set; }
        = string.Empty;

    public DateTime DateOfBirth { get; set; }

    public Gender Gender { get; set; }

    public string IdentityCard { get; set; }
        = string.Empty;

    public string Email { get; set; }
        = string.Empty;

    public string PhoneNumber { get; set; }
        = string.Empty;

    public string Address { get; set; }
        = string.Empty;

    public string AvatarUrl { get; set; }
        = string.Empty;

    public DateTime HireDate { get; set; }

    public DateTime? TerminationDate { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsDeleted { get; set; }

    public Guid DepartmentId { get; set; }

    public Department Department { get; set; }
        = null!;

    public Guid PositionId { get; set; }

    public Position Position { get; set; }
        = null!;

    public Guid UserId { get; set; }

    public User User { get; set; }
        = null!;

    public ICollection<BiometricProfile> BiometricProfiles
        { get; set; }
        = new List<BiometricProfile>();
        
    public ICollection<EmployeeSchedule> EmployeeSchedules
        { get; set; }
        = new List<EmployeeSchedule>();

    public ICollection<Attendance> Attendances
        { get; set; }
        = new List<Attendance>();

    public ICollection<LeaveRequest> LeaveRequests
        { get; set; }
        = new List<LeaveRequest>();

    public ICollection<Notification> Notifications
        { get; set; }
        = new List<Notification>();
}