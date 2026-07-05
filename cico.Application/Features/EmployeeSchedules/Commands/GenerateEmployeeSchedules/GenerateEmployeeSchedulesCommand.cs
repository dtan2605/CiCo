using MediatR;

namespace cico.Application.Features.EmployeeSchedules.Commands.GenerateEmployeeSchedules;

public record GenerateEmployeeSchedulesCommand(
    Guid ScheduleId,
    List<Guid> EmployeeIds,
    DateOnly FromDate,
    DateOnly ToDate,
    int DayOfWeekMask
) : IRequest<int>;
