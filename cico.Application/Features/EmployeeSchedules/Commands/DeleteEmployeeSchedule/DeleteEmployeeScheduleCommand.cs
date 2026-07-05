using MediatR;

namespace cico.Application.Features.EmployeeSchedules
    .Commands.DeleteEmployeeSchedule;

public record DeleteEmployeeScheduleCommand(
    Guid Id
) : IRequest;
