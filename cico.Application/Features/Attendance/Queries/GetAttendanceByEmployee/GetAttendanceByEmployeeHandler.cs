using AutoMapper;
using MediatR;
using cico.Application.DTOs.Attendances;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.Attendance.Queries.GetAttendanceByEmployee;

public class GetAttendanceByEmployeeHandler
    : IRequestHandler<
        GetAttendanceByEmployeeQuery,
        List<AttendanceDto>>
{
    private readonly IAttendanceRepository _repository;
    private readonly IMapper _mapper;

    public GetAttendanceByEmployeeHandler(
        IAttendanceRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<AttendanceDto>>
        Handle(
            GetAttendanceByEmployeeQuery request,
            CancellationToken cancellationToken)
    {
        var records =
            await _repository
                .GetAttendanceByEmployeeAsync(
                    request.EmployeeId);

        return _mapper.Map<List<AttendanceDto>>(
            records);
    }
}
