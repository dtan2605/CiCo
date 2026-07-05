using AutoMapper;
using MediatR;
using cico.Application.DTOs.Attendances;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.Attendance.Queries.GetAttendanceByDate;

public class GetAttendanceByDateHandler
    : IRequestHandler<
        GetAttendanceByDateQuery,
        List<AttendanceDto>>
{
    private readonly IAttendanceRepository _repository;
    private readonly IMapper _mapper;

    public GetAttendanceByDateHandler(
        IAttendanceRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<AttendanceDto>>
        Handle(
            GetAttendanceByDateQuery request,
            CancellationToken cancellationToken)
    {
        var records =
            await _repository
                .GetAttendanceByDateAsync(
                    request.Date);

        return _mapper.Map<List<AttendanceDto>>(
            records);
    }
}
