using MediatR;
using cico.Application.DTOs.Attendances;

namespace cico.Application.Features.Attendance.Queries.GetAttendanceById;

public record GetAttendanceByIdQuery(
    Guid Id
) : IRequest<AttendanceDto?>;
