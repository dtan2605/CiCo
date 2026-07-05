using cico.Domain.Common;
using cico.Domain.ValueObjects;

namespace cico.Domain.Entities;
public class User : AuditableEntity
{
    public string Username { get; set; }
        = string.Empty;

    public string PasswordHash { get; set; }
        = string.Empty;

    public string Email { get; set; }
        = string.Empty;

    public bool IsActive { get; set; } = true;

    public int FailedLoginCount { get; set; }

    public bool IsLocked { get; set; }

    public DateTime? LockoutEnd { get; set; }

    public string SecurityStamp { get; set; }
        = Guid.NewGuid().ToString();

    public DateTime? LastLoginAt { get; set; }

    public bool EmailConfirmed { get; set; }

    public string? EmailVerificationToken { get; set; }

    public string? PasswordResetToken { get; set; }

    public DateTime? PasswordResetExpiredAt { get; set; }

    public bool IsDeleted { get; set; }
    public Guid RoleId { get; set; }

    public Role Role { get; set; } = null!;

    public Employee? Employee { get; set; }

    public ICollection<RefreshToken> RefreshTokens
        { get; set; }
        = new List<RefreshToken>();
}