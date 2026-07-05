using MediatR;
using cico.Application.DTOs.AttendanceLogs;

namespace cico.Application.Features.AttendanceLogs
    .Queries.GetAttendanceLogsByAttendance;

public record GetAttendanceLogsByAttendanceQuery(
    Guid AttendanceId
) : IRequest<List<AttendanceLogDto>>;
