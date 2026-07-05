using AutoMapper;
using MediatR;
using cico.Application.DTOs.AttendanceLogs;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.AttendanceLogs
    .Queries.GetAttendanceLogsPaging;

public class GetAttendanceLogsPagingHandler
    : IRequestHandler<
        GetAttendanceLogsPagingQuery,
        List<AttendanceLogDto>>
{
    private readonly IAttendanceLogRepository _repository;
    private readonly IMapper _mapper;

    public GetAttendanceLogsPagingHandler(
        IAttendanceLogRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<AttendanceLogDto>> Handle(
        GetAttendanceLogsPagingQuery request,
        CancellationToken cancellationToken)
    {
        var all =
            await _repository.GetAllAsync();

        var filtered = all
            .Where(x =>
                (!request.AttendanceId.HasValue ||
                    x.AttendanceId ==
                        request.AttendanceId.Value) &&
                (!request.DeviceId.HasValue ||
                    x.DeviceId ==
                        request.DeviceId.Value) &&
                (!request.FromDate.HasValue ||
                    x.ScanTime >= request.FromDate.Value) &&
                (!request.ToDate.HasValue ||
                    x.ScanTime <= request.ToDate.Value) &&
                (!request.IsSuccess.HasValue ||
                    x.IsSuccess == request.IsSuccess.Value))
            .Skip((request.PageNumber - 1)
                * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return _mapper.Map<List<AttendanceLogDto>>(
            filtered);
    }
}
