using MediatR;
using cico.Application.DTOs.EmployeeSchedules;

namespace cico.Application.Features.EmployeeSchedules
    .Queries.GetEmployeeSchedulesPaging;

public record GetEmployeeSchedulesPagingQuery(
    int PageNumber = 1,
    int PageSize = 10,
    Guid? EmployeeId = null,
    Guid? ScheduleId = null,
    DateOnly? FromDate = null,
    DateOnly? ToDate = null
) : IRequest<List<EmployeeScheduleDto>>;
