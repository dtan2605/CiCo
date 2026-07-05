using AutoMapper;
using MediatR;
using cico.Application.DTOs.Schedules;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.Schedules.Queries.GetScheduleById;

public class GetScheduleByIdHandler
    : IRequestHandler<GetScheduleByIdQuery, ScheduleDto?>
{
    private readonly IScheduleRepository _repository;
    private readonly IMapper _mapper;

    public GetScheduleByIdHandler(
        IScheduleRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ScheduleDto?> Handle(
        GetScheduleByIdQuery request,
        CancellationToken cancellationToken)
    {
        var schedule =
            await _repository.GetByIdAsync(
                request.Id);

        if (schedule == null)
            return null;

        return _mapper.Map<ScheduleDto>(
            schedule);
    }
}
