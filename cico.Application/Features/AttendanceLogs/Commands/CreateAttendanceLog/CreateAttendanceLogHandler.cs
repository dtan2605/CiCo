using AutoMapper;
using MediatR;
using cico.Domain.Entities;
using cico.Application.DTOs.AttendanceLogs;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.AttendanceLogs
    .Commands.CreateAttendanceLog;

public class CreateAttendanceLogHandler
    : IRequestHandler<
        CreateAttendanceLogCommand,
        AttendanceLogDto>
{
    private readonly IAttendanceLogRepository _repository;
    private readonly IMapper _mapper;

    public CreateAttendanceLogHandler(
        IAttendanceLogRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<AttendanceLogDto> Handle(
        CreateAttendanceLogCommand request,
        CancellationToken cancellationToken)
    {
        var log = new AttendanceLog
        {
            ScanTime = request.ScanTime,
            IsSuccess = request.IsSuccess,
            Message = request.Message ?? string.Empty,
            AttendanceId = request.AttendanceId,
            DeviceId = request.DeviceId,
            Method = request.Method
        };

        await _repository.AddAsync(log);

        return _mapper.Map<AttendanceLogDto>(log);
    }
}
