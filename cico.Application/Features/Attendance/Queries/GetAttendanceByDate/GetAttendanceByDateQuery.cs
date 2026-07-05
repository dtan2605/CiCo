using MediatR;
using cico.Application.DTOs.Attendances;

namespace cico.Application.Features.Attendance.Queries.GetAttendanceByDate;

public record GetAttendanceByDateQuery(
    DateOnly Date
) : IRequest<List<AttendanceDto>>;
