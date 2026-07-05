using AutoMapper;
using MediatR;
using cico.Application.DTOs.AttendanceLogs;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.AttendanceLogs
    .Queries.GetAttendanceLogsByDevice;

public class GetAttendanceLogsByDeviceHandler
    : IRequestHandler<
        GetAttendanceLogsByDeviceQuery,
        List<AttendanceLogDto>>
{
    private readonly IAttendanceLogRepository _repository;
    private readonly IMapper _mapper;

    public GetAttendanceLogsByDeviceHandler(
        IAttendanceLogRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<AttendanceLogDto>> Handle(
        GetAttendanceLogsByDeviceQuery request,
        CancellationToken cancellationToken)
    {
        var logs =
            await _repository
                .GetByDeviceIdAsync(
                    request.DeviceId);

        return _mapper.Map<List<AttendanceLogDto>>(
            logs);
    }
}
