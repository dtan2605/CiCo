using cico.Domain.Entities;

namespace cico.Infrastructure.Persistence.SeedData;

public static class DepartmentSeed
{
    public static IEnumerable<Department> Get()
    {
        return new List<Department>
        {
            new()
            {
                Id = Guid.Parse(
                    "AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA"),
                Name = "Human Resources"
            },
            new()
            {
                Id = Guid.Parse(
                    "BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB"),
                Name = "Information Technology"
            },
            new()
            {
                Id = Guid.Parse(
                    "CCCCCCCC-CCCC-CCCC-CCCC-CCCCCCCCCCCC"),
                Name = "Finance"
            }
        };
    }
}
