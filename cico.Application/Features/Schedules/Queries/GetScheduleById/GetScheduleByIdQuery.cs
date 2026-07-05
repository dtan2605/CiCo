using MediatR;
using cico.Application.DTOs.Schedules;

namespace cico.Application.Features.Schedules.Queries.GetScheduleById;

public record GetScheduleByIdQuery(
    Guid Id
) : IRequest<ScheduleDto?>;
