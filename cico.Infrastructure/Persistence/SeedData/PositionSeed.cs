using cico.Domain.Entities;

namespace cico.Infrastructure.Persistence.SeedData;

public static class PositionSeed
{
    public static IEnumerable<Position> Get()
    {
        return new List<Position>
        {
            new()
            {
                Id = Guid.Parse(
                    "DDDDDDDD-DDDD-DDDD-DDDD-DDDDDDDDDDDD"),
                Name = "Staff"
            },
            new()
            {
                Id = Guid.Parse(
                    "EEEEEEEE-EEEE-EEEE-EEEE-EEEEEEEEEEEE"),
                Name = "Team Lead"
            },
            new()
            {
                Id = Guid.Parse(
                    "FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF"),
                Name = "Manager"
            }
        };
    }
}
