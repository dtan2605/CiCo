using MediatR;
using cico.Application.DTOs.Attendances;

namespace cico.Application.Features.Attendance.Queries.GetAttendanceByEmployee;

public record GetAttendanceByEmployeeQuery(
    Guid EmployeeId
) : IRequest<List<AttendanceDto>>;
