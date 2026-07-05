using cico.Domain.Common;

namespace cico.Domain.Entities;

public class RefreshToken : AuditableEntity
{
    public string Token { get; set; } = string.Empty;

    public DateTime Expires { get; set; }

    public bool IsRevoked { get; set; }

    public bool IsUsed { get; set; }

    public Guid UserId { get; set; }

    public User User { get; set; } = null!;
}