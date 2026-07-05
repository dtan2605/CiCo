using MediatR;
using cico.Application.DTOs.EmployeeSchedules;

namespace cico.Application.Features.EmployeeSchedules
    .Commands.CreateEmployeeSchedule;

public record CreateEmployeeScheduleCommand(
    Guid EmployeeId,
    Guid ScheduleId,
    DateOnly WorkDate,
    bool IsOvertime,
    string? Note
) : IRequest<EmployeeScheduleDto>;
