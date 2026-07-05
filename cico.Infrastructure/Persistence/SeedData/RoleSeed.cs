using cico.Domain.Entities;

namespace cico.Infrastructure.Persistence.SeedData;

public static class RoleSeed
{
    public static IEnumerable<Role> Get()
    {
        return new List<Role>
        {
            new()
            {
                Id = Guid.Parse(
                    "11111111-1111-1111-1111-111111111111"),
                Name = "Admin"
            },

            new()
            {
                Id = Guid.Parse(
                    "22222222-2222-2222-2222-222222222222"),
                Name = "Manager"
            },

            new()
            {
                Id = Guid.Parse(
                    "33333333-3333-3333-3333-333333333333"),
                Name = "Employee"
            }
        };
    }
}