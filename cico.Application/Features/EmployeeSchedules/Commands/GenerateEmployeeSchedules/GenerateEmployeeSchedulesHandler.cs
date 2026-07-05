using MediatR;
using cico.Domain.Entities;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.EmployeeSchedules.Commands.GenerateEmployeeSchedules;

public class GenerateEmployeeSchedulesHandler
    : IRequestHandler<GenerateEmployeeSchedulesCommand, int>
{
    private readonly IEmployeeScheduleRepository _repo;

    public GenerateEmployeeSchedulesHandler(
        IEmployeeScheduleRepository repo)
    {
        _repo = repo;
    }

    public async Task<int> Handle(
        GenerateEmployeeSchedulesCommand request,
        CancellationToken cancellationToken)
    {
        var created = 0;
        var current = request.FromDate;

        while (current <= request.ToDate)
        {
            var dayBit = 1 << ((int)current.DayOfWeek);
            if ((request.DayOfWeekMask & dayBit) == 0)
            {
                current = current.AddDays(1);
                continue;
            }

            foreach (var employeeId in request.EmployeeIds)
            {
                var exists = await _repo.ExistsAsync(
                    employeeId, request.ScheduleId, current);

                if (exists)
                    continue;

                var entity = new EmployeeSchedule
                {
                    EmployeeId = employeeId,
                    ScheduleId = request.ScheduleId,
                    WorkDate = current
                };

                await _repo.AddAsync(entity);
                created++;
            }

            current = current.AddDays(1);
        }

        return created;
    }
}
