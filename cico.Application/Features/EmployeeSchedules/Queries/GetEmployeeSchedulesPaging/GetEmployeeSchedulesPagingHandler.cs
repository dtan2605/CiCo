using AutoMapper;
using MediatR;
using cico.Application.DTOs.EmployeeSchedules;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.EmployeeSchedules
    .Queries.GetEmployeeSchedulesPaging;

public class GetEmployeeSchedulesPagingHandler
    : IRequestHandler<
        GetEmployeeSchedulesPagingQuery,
        List<EmployeeScheduleDto>>
{
    private readonly IEmployeeScheduleRepository _repository;
    private readonly IMapper _mapper;

    public GetEmployeeSchedulesPagingHandler(
        IEmployeeScheduleRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<EmployeeScheduleDto>> Handle(
        GetEmployeeSchedulesPagingQuery request,
        CancellationToken cancellationToken)
    {
        var all =
            await _repository.GetAllAsync();

        var filtered = all
            .Where(x =>
                (!request.EmployeeId.HasValue ||
                    x.EmployeeId == request.EmployeeId.Value) &&
                (!request.ScheduleId.HasValue ||
                    x.ScheduleId == request.ScheduleId.Value) &&
                (!request.FromDate.HasValue ||
                    x.WorkDate >= request.FromDate.Value) &&
                (!request.ToDate.HasValue ||
                    x.WorkDate <= request.ToDate.Value))
            .OrderByDescending(x => x.WorkDate)
            .Skip((request.PageNumber - 1)
                * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return _mapper.Map<List<EmployeeScheduleDto>>(
            filtered);
    }
}
