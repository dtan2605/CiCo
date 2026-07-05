using AutoMapper;
using MediatR;
using cico.Application.DTOs.Schedules;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.Schedules.Queries.GetSchedulesPaging;

public class GetSchedulesPagingHandler
    : IRequestHandler<
        GetSchedulesPagingQuery,
        List<ScheduleDto>>
{
    private readonly IScheduleRepository _repository;
    private readonly IMapper _mapper;

    public GetSchedulesPagingHandler(
        IScheduleRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<ScheduleDto>>
        Handle(
            GetSchedulesPagingQuery request,
            CancellationToken cancellationToken)
    {
        var all =
            await _repository.GetAllAsync();

        var filtered = all
            .Where(x =>
                string.IsNullOrEmpty(request.Keyword) ||
                x.Name.Contains(
                    request.Keyword,
                    StringComparison.OrdinalIgnoreCase))
            .Skip((request.PageNumber - 1)
                * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return _mapper.Map<List<ScheduleDto>>(
            filtered);
    }
}
