using cico.Domain.Common;

namespace cico.Domain.Entities;

public class Department : AuditableEntity
{
    public string Name { get; set; }
        = string.Empty;

    public string Description { get; set; }
        = string.Empty;

    public ICollection<Employee> Employees
        { get; set; }
        = new List<Employee>();
}