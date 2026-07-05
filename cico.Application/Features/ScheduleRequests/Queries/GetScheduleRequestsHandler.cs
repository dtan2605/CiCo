using AutoMapper;
using MediatR;
using cico.Application.Common.Interfaces.Persistence;
using cico.Domain.Enums;

namespace cico.Application.Features.ScheduleRequests.Queries;

public class GetScheduleRequestsHandler : IRequestHandler<GetScheduleRequestsQuery, List<ScheduleRequestDto>>
{
    private readonly IScheduleRequestRepository _repo;
    private readonly IMapper _mapper;

    public GetScheduleRequestsHandler(IScheduleRequestRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<List<ScheduleRequestDto>> Handle(GetScheduleRequestsQuery request, CancellationToken cancellationToken)
    {
        ScheduleRequestStatus? status = request.Status.HasValue ? (ScheduleRequestStatus)request.Status.Value : null;
        var items = await _repo.GetPagingAsync(request.PageNumber, request.PageSize, request.EmployeeId, status);
        return _mapper.Map<List<ScheduleRequestDto>>(items);
    }
}
