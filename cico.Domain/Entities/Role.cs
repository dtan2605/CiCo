using cico.Domain.Common;

namespace cico.Domain.Entities;
public class Role : AuditableEntity
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; }
        = string.Empty;

    public ICollection<User> Users { get; set; }
        = new List<User>();
    
    public ICollection<RolePermission> RolePermissions { get; set; }
        = new List<RolePermission>();
}