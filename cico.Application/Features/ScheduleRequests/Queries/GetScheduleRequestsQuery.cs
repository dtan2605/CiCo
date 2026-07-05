using MediatR;

namespace cico.Application.Features.ScheduleRequests.Queries;

public record GetScheduleRequestsQuery(
    int PageNumber = 1,
    int PageSize = 20,
    Guid? EmployeeId = null,
    int? Status = null
) : IRequest<List<ScheduleRequestDto>>;
