using MediatR;
using cico.Application.DTOs.Schedules;

namespace cico.Application.Features.Schedules.Queries.GetSchedulesPaging;

public record GetSchedulesPagingQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? Keyword = null
) : IRequest<List<ScheduleDto>>;
