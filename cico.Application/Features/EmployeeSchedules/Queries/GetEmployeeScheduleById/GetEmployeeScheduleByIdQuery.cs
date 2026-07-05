using MediatR;
using cico.Application.DTOs.EmployeeSchedules;

namespace cico.Application.Features.EmployeeSchedules
    .Queries.GetEmployeeScheduleById;

public record GetEmployeeScheduleByIdQuery(
    Guid Id
) : IRequest<EmployeeScheduleDto?>;
