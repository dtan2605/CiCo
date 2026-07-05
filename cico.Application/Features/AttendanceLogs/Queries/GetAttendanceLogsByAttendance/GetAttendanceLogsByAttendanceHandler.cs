using AutoMapper;
using MediatR;
using cico.Application.DTOs.AttendanceLogs;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.AttendanceLogs
    .Queries.GetAttendanceLogsByAttendance;

public class GetAttendanceLogsByAttendanceHandler
    : IRequestHandler<
        GetAttendanceLogsByAttendanceQuery,
        List<AttendanceLogDto>>
{
    private readonly IAttendanceLogRepository _repository;
    private readonly IMapper _mapper;

    public GetAttendanceLogsByAttendanceHandler(
        IAttendanceLogRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<AttendanceLogDto>> Handle(
        GetAttendanceLogsByAttendanceQuery request,
        CancellationToken cancellationToken)
    {
        var logs =
            await _repository
                .GetByAttendanceIdAsync(
                    request.AttendanceId);

        return _mapper.Map<List<AttendanceLogDto>>(
            logs);
    }
}
