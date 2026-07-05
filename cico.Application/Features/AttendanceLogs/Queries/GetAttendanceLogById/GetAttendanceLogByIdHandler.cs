using AutoMapper;
using MediatR;
using cico.Application.DTOs.AttendanceLogs;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.AttendanceLogs
    .Queries.GetAttendanceLogById;

public class GetAttendanceLogByIdHandler
    : IRequestHandler<
        GetAttendanceLogByIdQuery,
        AttendanceLogDto?>
{
    private readonly IAttendanceLogRepository _repository;
    private readonly IMapper _mapper;

    public GetAttendanceLogByIdHandler(
        IAttendanceLogRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<AttendanceLogDto?> Handle(
        GetAttendanceLogByIdQuery request,
        CancellationToken cancellationToken)
    {
        var log =
            await _repository.GetByIdAsync(
                request.Id);

        if (log == null)
            return null;

        return _mapper.Map<AttendanceLogDto>(log);
    }
}
