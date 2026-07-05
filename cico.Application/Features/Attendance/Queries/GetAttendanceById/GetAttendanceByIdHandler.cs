using AutoMapper;
using MediatR;
using cico.Application.DTOs.Attendances;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.Attendance.Queries.GetAttendanceById;

public class GetAttendanceByIdHandler
    : IRequestHandler<GetAttendanceByIdQuery, AttendanceDto?>
{
    private readonly IAttendanceRepository _repository;
    private readonly IMapper _mapper;

    public GetAttendanceByIdHandler(
        IAttendanceRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<AttendanceDto?> Handle(
        GetAttendanceByIdQuery request,
        CancellationToken cancellationToken)
    {
        var attendance =
            await _repository.GetByIdAsync(
                request.Id);

        if (attendance == null)
            return null;

        return _mapper.Map<AttendanceDto>(
            attendance);
    }
}
