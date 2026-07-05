using cico.Domain.Entities;

namespace cico.Infrastructure.Persistence.SeedData;

public static class ScheduleSeed
{
    public static IEnumerable<Schedule> Get()
    {
        return new List<Schedule>
        {
            new()
            {
                Id = Guid.Parse(
                    "11111111-2222-3333-4444-555555555555"),
                Name = "Morning Shift",
                StartTime = new TimeOnly(7, 0),
                EndTime = new TimeOnly(15, 0),
                LateThresholdMinutes = 15
            },
            new()
            {
                Id = Guid.Parse(
                    "66666666-7777-8888-9999-000000000000"),
                Name = "Afternoon Shift",
                StartTime = new TimeOnly(15, 0),
                EndTime = new TimeOnly(23, 0),
                LateThresholdMinutes = 15
            }
        };
    }
}
